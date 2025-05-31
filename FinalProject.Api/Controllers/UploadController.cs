
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.TranscribeService;
using Amazon.TranscribeService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

[ApiController]
[Route("upload")]
public class UploadController : ControllerBase
{
    private readonly IAmazonS3 _s3Client;
    private readonly IAmazonTranscribeService _transcribeService;
    private readonly string _bucketName;

    public UploadController(IConfiguration config)
    {
        var awsAccessKey = config["AWS_ACCESS_KEY"];
        var awsSecretKey = config["AWS_SECRET_KEY"];
        var region = Amazon.RegionEndpoint.GetBySystemName(config["AWS_REGION"]);
        _bucketName = config["BUCKET_NAME"];

        var credentials = new Amazon.Runtime.BasicAWSCredentials(awsAccessKey, awsSecretKey);
        _s3Client = new AmazonS3Client(credentials, region);
        _transcribeService = new AmazonTranscribeServiceClient(credentials, region);
    }

    //[Authorize(Policy = "TeacherOrAdmin")]
    [HttpPost("upload-file")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        try
        {
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = file.FileName,
                InputStream = file.OpenReadStream(),
                ContentType = file.ContentType
            };

            await _s3Client.PutObjectAsync(request);

            var transcriptionJobName = file.FileName + "-transcription";
            var transcriptionRequest = new StartTranscriptionJobRequest
            {
                TranscriptionJobName = transcriptionJobName,
                LanguageCode = "he-IL",
                Media = new Media
                {
                    MediaFileUri = $"https://s3.{_s3Client.Config.RegionEndpoint.SystemName}.amazonaws.com/{_bucketName}/{file.FileName}"
                },
                OutputBucketName = _bucketName
            };

            await _transcribeService.StartTranscriptionJobAsync(transcriptionRequest);

            return Ok("File uploaded and transcription started successfully.");
        }
        catch (AmazonS3Exception s3Ex)
        {
            return StatusCode((int)s3Ex.StatusCode, $"S3 error: {s3Ex.Message}");
        }
        catch (AmazonTranscribeServiceException transcribeEx)
        {
            return StatusCode((int)transcribeEx.StatusCode, $"Transcribe error: {transcribeEx.Message}");
        }
    }

    [Authorize]
    [HttpGet("transcript")]
    [ProducesResponseType(typeof(string), 200)] // Swagger יציג את זה כמחרוזת
    public async Task<IActionResult> GetTranscriptContentAsync([FromQuery] string fileName)
    {
        try
        {
            // בדיקה אם הקובץ קיים
            var metadataRequest = new GetObjectMetadataRequest
            {
                BucketName = _bucketName,
                Key = fileName
            };

            try
            {
                await _s3Client.GetObjectMetadataAsync(metadataRequest);
            }
            catch (AmazonS3Exception e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound("Transcript file not found.");
            }

            // הורדת הקובץ
            var getRequest = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName
            };

            using var response = await _s3Client.GetObjectAsync(getRequest);
            using var reader = new StreamReader(response.ResponseStream);
            var content = await reader.ReadToEndAsync();

            // חילוץ טקסט מתוך transcripts
            try
            {
                var jsonNode = JsonNode.Parse(content);
                var transcriptsArray = jsonNode?["results"]?["transcripts"]?.AsArray();

                if (transcriptsArray == null || transcriptsArray.Count == 0)
                    return NotFound("No transcripts found in the file.");

                var combinedText = string.Join(" ", transcriptsArray
                    .Select(t => t?["transcript"]?.ToString())
                    .Where(t => !string.IsNullOrWhiteSpace(t)));

                if (string.IsNullOrWhiteSpace(combinedText))
                    return NotFound("Transcript text is empty.");

                return Ok(combinedText); // מחזיר רק את הטקסט במחרוזת אחת
            }
            catch (JsonException)
            {
                return BadRequest("The file content is not a valid JSON.");
            }
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            return StatusCode(403, "Access Denied: Check permissions.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }
}
