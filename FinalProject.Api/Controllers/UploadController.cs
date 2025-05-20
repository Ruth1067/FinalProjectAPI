////////using Microsoft.AspNetCore.Http;
////////using Microsoft.AspNetCore.Mvc;
////////using Amazon.S3;
////////using Amazon.S3.Model;
////////using System.Threading.Tasks;
////////using Microsoft.AspNetCore.Authorization;

////////namespace FinalProject.Api.Controllers
////////{
////////    [ApiController]
////////    [Route("api/upload")]
////////    public class UploadController : ControllerBase
////////    {
////////        private readonly IAmazonS3 _s3Client;

////////        public UploadController(IAmazonS3 s3Client)
////////        {
////////            _s3Client = s3Client;
////////        }

////////        [HttpPost("upload-file")]
////////        //[Authorize(Policy = "TeacherOrAdmin")]
////////        public async Task<IActionResult> UploadFile(IFormFile file)
////////        {
////////            if (file == null || file.Length == 0)
////////            {
////////                return BadRequest("No file uploaded.");
////////            }

////////            // זיהוי סוג הקובץ
////////            var contentType = file.ContentType;

////////            // בדיקות נוספות על סוג הקובץ
////////            if (contentType != "image/jpeg" && contentType != "image/png" &&
////////                contentType != "application/pdf" &&
////////                contentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
////////            {
////////                return BadRequest("Unsupported file type.");
////////            }

////////            // כאן תוכל להעלות את הקובץ ל-S3
////////            var request = new PutObjectRequest
////////            {
////////                BucketName = "adminpermission", // שם הדלי שלך
////////                Key = file.FileName,
////////                InputStream = file.OpenReadStream(),
////////                ContentType = contentType
////////            };

////////            await _s3Client.PutObjectAsync(request);

////////            return Ok("File uploaded successfully.");
////////        }
////////    }
////////}

////////using Microsoft.AspNetCore.Http;
////////using Microsoft.AspNetCore.Mvc;
////////using Amazon.S3;
////////using Amazon.S3.Model;
////////using System.Threading.Tasks;
////////using Microsoft.AspNetCore.Authorization;

////////namespace FinalProject.Api.Controllers
////////{
////////    [ApiController]
////////    [Route("api/upload")]
////////    public class UploadController : ControllerBase
////////    {
////////        private readonly IAmazonS3 _s3Client;
////////        //private const string BucketName = "adminpermission";

////////        public UploadController(IAmazonS3 s3Client)
////////        {
////////            _s3Client = s3Client;
////////        }

////////        [HttpPost("upload-file")]
////////        //[Authorize(Policy = "TeacherOrAdmin")]
////////        public async Task<IActionResult> UploadFile(IFormFile file)
////////        {
////////            if (file == null || file.Length == 0)
////////            {
////////                return BadRequest("No file uploaded.");
////////            }

////////            // כאן תוכל להעלות את הקובץ ל-S3
////////            var request = new PutObjectRequest
////////            {
////////                BucketName = "adminpermission", // שם הדלי שלך
////////                Key = file.FileName,
////////                InputStream = file.OpenReadStream(),
////////                ContentType = file.ContentType // נשמור את סוג הקובץ כפי שהוא
////////            };

////////            await _s3Client.PutObjectAsync(request);

////////            return Ok("File uploaded successfully.");
////////        }


////////        //[HttpGet("download-file/{fileName}")]
////////        //public async Task<IActionResult> DownloadFile(string fileName)
////////        //{
////////        //    try
////////        //    {
////////        //        var request = new GetObjectRequest
////////        //        {
////////        //            BucketName = "adminpermission", // שם הדלי שלך
////////        //            Key = fileName
////////        //        };

////////        //        using (var response = await _s3Client.GetObjectAsync(request))
////////        //        using (var responseStream = response.ResponseStream)
////////        //        {
////////        //            var memoryStream = new MemoryStream();
////////        //            await responseStream.CopyToAsync(memoryStream);
////////        //            memoryStream.Position = 0; // לאתחל את המיקום של ה-stream

////////        //            // קביעת סוג תוכן על בסיס סיומת הקובץ
////////        //            var contentType = GetContentType(fileName);

////////        //            // החזרת הקובץ עם סוג תוכן
////////        //            return File(memoryStream.ToArray(), contentType, fileName);
////////        //        }
////////        //    }
////////        //    catch (AmazonS3Exception e)
////////        //    {
////////        //        return NotFound("שגיאה בהורדת הקובץ: " + e.Message);
////////        //    }
////////        //    catch (Exception e)
////////        //    {
////////        //        return StatusCode(500, "שגיאה בלתי צפויה: " + e.Message);
////////        //    }
////////        //}

////////        //private string GetContentType(string fileName)
////////        //{
////////        //    var extension = Path.GetExtension(fileName).ToLowerInvariant();
////////        //    return extension switch
////////        //    {
////////        //        ".pdf" => "application/pdf",
////////        //        ".jpg" => "image/jpeg",
////////        //        ".jpeg" => "image/jpeg",
////////        //        ".png" => "image/png",
////////        //        ".txt" => "text/plain",
////////        //        // הוסף סוגי תוכן נוספים לפי הצורך
////////        //        _ => "application/octet-stream", // סוג תוכן ברירת מחדל
////////        //    };
////////        //}

////////        [HttpGet]
////////        public async Task<IActionResult> GetFiles()
////////        {
////////            var request = new ListObjectsV2Request
////////            {
////////                BucketName = "adminpermission"
////////            };

////////            var response = await _s3Client.ListObjectsV2Async(request);
////////            var files = response.S3Objects.Select(o => o.Key).ToList();

////////            return Ok(files);
////////        }
////////    }

////////[HttpGet("download-file/{fileName}")]
////////public async Task<IActionResult> DownloadFile(string fileName)
////////{
////////    try
////////    {
////////        var request = new GetObjectRequest
////////        {
////////            BucketName = "adminpermission", // שם הדלי שלך
////////            Key = fileName
////////        };

////////        using (var response = await _s3Client.GetObjectAsync(request))
////////        using (var responseStream = response.ResponseStream)
////////        {
////////            var memoryStream = new MemoryStream();
////////            await responseStream.CopyToAsync(memoryStream);
////////            memoryStream.Position = 0; // לאתחל את המיקום של ה-stream

////////            // החזרת הקובץ באמצעות FileContentResult
////////            return new FileContentResult(memoryStream.ToArray(), response.ContentType)
////////            {
////////                FileDownloadName = fileName // הגדרת שם הקובץ להורדה
////////            };
////////        }
////////    }
////////    catch (AmazonS3Exception e)
////////    {
////////        return NotFound("שגיאה בהורדת הקובץ: " + e.Message);
////////    }
////////    catch (Exception e)
////////    {
////////        return StatusCode(500, "שגיאה בלתי צפויה: " + e.Message);
////////    }
////////}
////////}

//////using Microsoft.AspNetCore.Http;
//////using Microsoft.AspNetCore.Mvc;
//////using Amazon.S3;
//////using Amazon.S3.Model;
////////using Amazon.TranscribeService;
////////using System.Threading.Tasks;
////////using System.Linq;
////////using Amazon.TranscribeService.Model;
////////namespace FinalProject.Api.Controllers
////////{
////////    [ApiController]
////////    [Route("api/upload")]
////////    public class UploadController : ControllerBase
////////    {
////////        private readonly IAmazonS3 _s3Client;
////////        private readonly IAmazonTranscribeService _transcribeService;

////////        public UploadController(IAmazonS3 s3Client, IAmazonTranscribeService transcribeService)
////////        {
////////            _s3Client = s3Client;
////////            _transcribeService = transcribeService;
////////        }

////////        [HttpPost("upload-file")]
////////        public async Task<IActionResult> UploadFile(IFormFile file)
////////        {
////////            if (file == null || file.Length == 0)
////////            {
////////                return BadRequest("No file uploaded.");
////////            }

////////            var request = new PutObjectRequest
////////            {
////////                BucketName = "adminpermission", // שם הדלי שלך
////////                Key = file.FileName,
////////                InputStream = file.OpenReadStream(),
////////                ContentType = file.ContentType // נשמור את סוג הקובץ כפי שהוא
////////            };

////////            await _s3Client.PutObjectAsync(request);

////////            // התחלת תהליך התמלול
////////            var transcriptionJobName = file.FileName + "-transcription";
////////            var transcriptionRequest = new StartTranscriptionJobRequest
////////            {
////////                TranscriptionJobName = transcriptionJobName,
////////                LanguageCode = "he-IL", // שנה לשפה המתאימה
////////                Media = new Media
////////                {
////////                    MediaFileUri = $"https://s3.amazonaws.com/adminpermission/{file.FileName}" // URL לקובץ ב-S3
////////                },
////////                OutputBucketName = "adminpermission" // דלי פלט לתוצאות התמלול
////////            };

////////            await _transcribeService.StartTranscriptionJobAsync(transcriptionRequest);

////////            return Ok("File uploaded and transcription started successfully.");
////////        }

////////        [HttpGet]
////////        public async Task<IActionResult> GetFiles()
////////        {
////////            var request = new ListObjectsV2Request
////////            {
////////                BucketName = "adminpermission"
////////            };

////////            var response = await _s3Client.ListObjectsV2Async(request);
////////            var files = response.S3Objects.Select(o => o.Key).ToList();

////////            return Ok(files);
////////        }
////////    }
////////}
////////using Amazon.S3.Model;
////////using Amazon.S3;
////////using Amazon.TranscribeService.Model;
////////using Amazon.TranscribeService;
////////using Microsoft.AspNetCore.Mvc;

////////    [HttpPost("upload-file")]
////////    public async Task<IActionResult> UploadFile(IFormFile file)
////////    {
////////        if (file == null || file.Length == 0)
////////        {
////////            return BadRequest("No file uploaded."); // 'BadRequest' is part of ControllerBase
////////        }

////////        try
////////        {
////////            var request = new PutObjectRequest
////////            {
////////                BucketName = "adminpermission",
////////                Key = file.FileName,
////////                InputStream = file.OpenReadStream(),
////////                ContentType = file.ContentType
////////            };

////////            await _s3Client.PutObjectAsync(request);

////////            // Start transcription process
////////            var transcriptionJobName = file.FileName + "-transcription";
////////            var transcriptionRequest = new StartTranscriptionJobRequest
////////            {
////////                TranscriptionJobName = transcriptionJobName,
////////                LanguageCode = "he-IL",
////////                Media = new Media
////////                {
////////                    MediaFileUri = $"https://s3.amazonaws.com/adminpermission/{file.FileName}"
////////                },
////////                OutputBucketName = "adminpermission"
////////            };

////////            await _transcribeService.StartTranscriptionJobAsync(transcriptionRequest);

////////            return Ok("File uploaded and transcription started successfully."); // 'Ok' is part of ControllerBase
////////        }
////////        catch (AmazonS3Exception s3Ex)
////////        {
////////            return StatusCode((int)s3Ex.StatusCode, $"S3 error: {s3Ex.Message}");
////////        }
////////        catch (AmazonTranscribeServiceException transcribeEx)
////////        {
////////            return StatusCode((int)transcribeEx.StatusCode, $"Transcribe error: {transcribeEx.Message}");
////////        }
////////        catch (Exception ex)
////////        {
////////            return StatusCode(500, $"Internal server error: {ex.Message}");
////////        }
////////    }
////////}
////////}
//////using Amazon.TranscribeService;
//////using Amazon.TranscribeService.Model;
//////using Microsoft.AspNetCore.Http;
//////using Microsoft.AspNetCore.Mvc;
//////using System.Threading.Tasks;
//////using Newtonsoft.Json.Linq;
//////using System.Net;

//////[ApiController]
//////[Route("upload")]
//////public class UploadController : ControllerBase
//////{
//////    private readonly IAmazonS3 _s3Client;
//////    private readonly IAmazonTranscribeService _transcribeService;

//////    public UploadController(IAmazonS3 s3Client, IAmazonTranscribeService transcribeService)
//////    {
//////        _s3Client = s3Client;
//////        _transcribeService = transcribeService;
//////    }

//////    [HttpPost("upload-file")]
//////    public async Task<IActionResult> UploadFile(IFormFile file)
//////    {
//////        if (file == null || file.Length == 0)
//////        {
//////            return BadRequest("No file uploaded.");
//////        }

//////        try
//////        {
//////            var request = new PutObjectRequest
//////            {
//////                BucketName = "adminpermission",
//////                Key = file.FileName,
//////                InputStream = file.OpenReadStream(),
//////                ContentType = file.ContentType
//////            };

//////            await _s3Client.PutObjectAsync(request);

//////            // Start transcription process
//////            var transcriptionJobName = file.FileName + "-transcription";
//////            var transcriptionRequest = new StartTranscriptionJobRequest
//////            {
//////                TranscriptionJobName = transcriptionJobName,
//////                LanguageCode = "he-IL",
//////                Media = new Media
//////                {
//////                    MediaFileUri = $"https://s3.amazonaws.com/adminpermission/{file.FileName}"
//////                },
//////                OutputBucketName = "adminpermission"
//////            };

//////            await _transcribeService.StartTranscriptionJobAsync(transcriptionRequest);

//////            return Ok("File uploaded and transcription started successfully.");
//////        }
//////        catch (AmazonS3Exception s3Ex)
//////        {
//////            return StatusCode((int)s3Ex.StatusCode, $"S3 error: {s3Ex.Message}");
//////        }
//////        catch (AmazonTranscribeServiceException transcribeEx)
//////        {
//////            return StatusCode((int)transcribeEx.StatusCode, $"Transcribe error: {transcribeEx.Message}");
//////        }
//////        // Remove this redundant catch block for `Exception` since it already covers all exceptions.
//////    }

//////    [HttpGet("transcript")]
//////    public async Task<IActionResult> GetTranscriptAsync([FromQuery] string fileName)
//////    {
//////        var client = new AmazonS3Client(Amazon.RegionEndpoint.EUNorth1);
//////        var bucketName = "adminpermission";

//////        try
//////        {
//////            var response = await client.GetObjectAsync(bucketName, fileName);
//////            using (var reader = new StreamReader(response.ResponseStream))
//////            {
//////                var content = await reader.ReadToEndAsync();
//////                return Content(content, "application/json");
//////            }
//////        }
//////        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
//////        {
//////            return StatusCode(403, "Access Denied: Check permissions.");
//////        }
//////    }

//////    //[HttpGet("transcription-results/{jobName}")]
//////    //public async Task<IActionResult> GetTranscriptionResults(string jobName)
//////    //{
//////    //    try
//////    //    {
//////    //        var request = new GetTranscriptionJobRequest
//////    //        {
//////    //            TranscriptionJobName = jobName
//////    //        };

//////    //        var response = await _transcribeService.GetTranscriptionJobAsync(request);
//////    //        var transcriptionJob = response.TranscriptionJob;

//////    //        if (transcriptionJob == null)
//////    //        {
//////    //            return NotFound("Transcription job not found.");
//////    //        }

//////    //        // Check the job status
//////    //        if (transcriptionJob.TranscriptionJobStatus == TranscriptionJobStatus.COMPLETED)
//////    //        {
//////    //            return Ok(transcriptionJob.Transcript);
//////    //        }
//////    //        else if (transcriptionJob.TranscriptionJobStatus == TranscriptionJobStatus.FAILED)
//////    //        {
//////    //            return BadRequest($"Transcription job failed: {transcriptionJob.FailureReason}");
//////    //        }

//////    //        return Ok("Transcription job is still in progress.");
//////    //    }
//////    //    catch (AmazonTranscribeServiceException transcribeEx)
//////    //    {
//////    //        return StatusCode((int)transcribeEx.StatusCode, $"Transcribe error: {transcribeEx.Message}");
//////    //    }
//////    //    catch (Exception ex)
//////    //    {
//////    //        return StatusCode(500, $"Internal server error: {ex.Message}");
//////    //    }
//////    //}
//////    //[HttpGet("transcription-results/{jobName}")]
//////    //public async Task<IActionResult> GetTranscriptionResults(string jobName)
//////    //{
//////    //    try
//////    //    {
//////    //        var request = new GetTranscriptionJobRequest
//////    //        {
//////    //            TranscriptionJobName = jobName
//////    //        };

//////    //        var response = await _transcribeService.GetTranscriptionJobAsync(request);
//////    //        var transcriptionJob = response.TranscriptionJob;

//////    //        if (transcriptionJob == null)
//////    //        {
//////    //            return NotFound("Transcription job not found.");
//////    //        }

//////    //        // Check the job status
//////    //        if (transcriptionJob.TranscriptionJobStatus == TranscriptionJobStatus.COMPLETED)
//////    //        {
//////    //            // Read the transcript file URI
//////    //            var transcriptFileUri = transcriptionJob.Transcript.TranscriptFileUri;

//////    //            // Fetch the transcript content
//////    //            using (HttpClient client = new HttpClient())
//////    //            {
//////    //                var transcriptResponse = await client.GetAsync(transcriptFileUri);

//////    //                if (!transcriptResponse.IsSuccessStatusCode)
//////    //                {
//////    //                    return StatusCode((int)transcriptResponse.StatusCode, $"Failed to fetch transcript: {transcriptResponse.ReasonPhrase}");
//////    //                }

//////    //                var transcriptContent = await transcriptResponse.Content.ReadAsStringAsync();
//////    //                var json = JObject.Parse(transcriptContent);

//////    //                // Extract the transcript text
//////    //                var transcriptionText = json["results"]["transcripts"][0]["transcript"].ToString();

//////    //                return Ok(transcriptionText);
//////    //            }
//////    //        }
//////    //        else if (transcriptionJob.TranscriptionJobStatus == TranscriptionJobStatus.FAILED)
//////    //        {
//////    //            return BadRequest($"Transcription job failed: {transcriptionJob.FailureReason}");
//////    //        }

//////    //        return Ok("Transcription job is still in progress.");
//////    //    }
//////    //    catch (AmazonTranscribeServiceException transcribeEx)
//////    //    {
//////    //        return StatusCode((int)transcribeEx.StatusCode, $"Transcribe error: {transcribeEx.Message}");
//////    //    }
//////    //    catch (Exception ex)
//////    //    {
//////    //        return StatusCode(500, $"Internal server error: {ex.Message}");
//////    //    }
//////    //}

//////    //..........................................
//////    //[HttpGet("transcription-results/{jobName}")]
//////    //public async Task<IActionResult> GetTranscriptionResults(string jobName)
//////    //{
//////    //    try
//////    //    {
//////    //        var request = new GetTranscriptionJobRequest
//////    //        {
//////    //            TranscriptionJobName = jobName
//////    //        };

//////    //        var response = await _transcribeService.GetTranscriptionJobAsync(request);
//////    //        var transcriptionJob = response.TranscriptionJob;

//////    //        if (transcriptionJob == null)
//////    //        {
//////    //            return NotFound("Transcription job not found.");
//////    //        }

//////    //        // Check the job status
//////    //        if (transcriptionJob.TranscriptionJobStatus == TranscriptionJobStatus.COMPLETED)
//////    //        {
//////    //            // Read the transcript file URI
//////    //            var transcriptFileUri = transcriptionJob.Transcript.TranscriptFileUri;
//////    //            if (string.IsNullOrEmpty(transcriptFileUri))
//////    //            {
//////    //                return BadRequest("Transcript file URI is empty.");
//////    //            }

//////    //            // Fetch the transcript content
//////    //            using (HttpClient client = new HttpClient())
//////    //            {
//////    //                Console.WriteLine($"Trying to access transcript at: {transcriptFileUri}");
//////    //                var transcriptResponse = await client.GetAsync(transcriptFileUri);

//////    //                if (!transcriptResponse.IsSuccessStatusCode)
//////    //                {
//////    //                    var content = await transcriptResponse.Content.ReadAsStringAsync();
//////    //                    Console.WriteLine($"Response content: {content}");
//////    //                    return StatusCode((int)transcriptResponse.StatusCode, $"Failed to fetch transcript: {transcriptResponse.ReasonPhrase}. Response content: {content}");
//////    //                }

//////    //                var transcriptContent = await transcriptResponse.Content.ReadAsStringAsync();
//////    //                var json = JObject.Parse(transcriptContent);

//////    //                // Extract the transcript text
//////    //                var transcriptionText = json["results"]["transcripts"][0]["transcript"].ToString();

//////    //                return Ok(transcriptionText);
//////    //            }
//////    //        }
//////    //        else if (transcriptionJob.TranscriptionJobStatus == TranscriptionJobStatus.FAILED)
//////    //        {
//////    //            return BadRequest($"Transcription job failed: {transcriptionJob.FailureReason}");
//////    //        }

//////    //        return Ok("Transcription job is still in progress.");
//////    //    }
//////    //    catch (AmazonTranscribeServiceException transcribeEx)
//////    //    {
//////    //        return StatusCode((int)transcribeEx.StatusCode, $"Transcribe error: {transcribeEx.Message}");
//////    //    }
//////    //    catch (Exception ex)
//////    //    {
//////    //        return StatusCode(500, $"Internal server error: {ex.Message}");
//////    //    }
//////    //}
//////    //................................
//////    //[HttpGet("transcription-results/{jobName}")]
//////    //public async Task<IActionResult> GetTranscriptionResults(string jobName)
//////    //{
//////    //    try
//////    //    {
//////    //        var request = new GetTranscriptionJobRequest
//////    //        {
//////    //            TranscriptionJobName = jobName
//////    //        };

//////    //        var response = await _transcribeService.GetTranscriptionJobAsync(request);
//////    //        var transcriptionJob = response.TranscriptionJob;

//////    //        if (transcriptionJob == null)
//////    //        {
//////    //            return NotFound("Transcription job not found.");
//////    //        }

//////    //        // Check the job status
//////    //        if (transcriptionJob.TranscriptionJobStatus == TranscriptionJobStatus.COMPLETED)
//////    //        {
//////    //            // Read the transcript file URI
//////    //            var transcriptFileUri = transcriptionJob.Transcript.TranscriptFileUri;
//////    //            if (string.IsNullOrEmpty(transcriptFileUri))
//////    //            {
//////    //                return BadRequest("Transcript file URI is empty.");
//////    //            }

//////    //            // Fetch the transcript content
//////    //            using (HttpClient client = new HttpClient())
//////    //            {
//////    //                var transcriptResponse = await client.GetAsync(transcriptFileUri);

//////    //                if (!transcriptResponse.IsSuccessStatusCode)
//////    //                {
//////    //                    // Log additional information for debugging
//////    //                    var content = await transcriptResponse.Content.ReadAsStringAsync();
//////    //                    return StatusCode((int)transcriptResponse.StatusCode, $"Failed to fetch transcript: {transcriptResponse.ReasonPhrase}. Response content: {content}");
//////    //                }

//////    //                var transcriptContent = await transcriptResponse.Content.ReadAsStringAsync();
//////    //                var json = JObject.Parse(transcriptContent);

//////    //                // Extract the transcript text
//////    //                var transcriptionText = json["results"]["transcripts"][0]["transcript"].ToString();

//////    //                return Ok(transcriptionText);
//////    //            }
//////    //        }
//////    //        else if (transcriptionJob.TranscriptionJobStatus == TranscriptionJobStatus.FAILED)
//////    //        {
//////    //            return BadRequest($"Transcription job failed: {transcriptionJob.FailureReason}");
//////    //        }

//////    //        return Ok("Transcription job is still in progress.");
//////    //    }
//////    //    catch (AmazonTranscribeServiceException transcribeEx)
//////    //    {
//////    //        return StatusCode((int)transcribeEx.StatusCode, $"Transcribe error: {transcribeEx.Message}");
//////    //    }
//////    //    catch (Exception ex)
//////    //    {
//////    //        return StatusCode(500, $"Internal server error: {ex.Message}");
//////    //    }
//////    //}

//////    //[HttpGet("transcription-results/{jobName}")]
//////    //public async Task<IActionResult> GetTranscriptionResults(string jobName)
//////    //{
//////    //    try
//////    //    {
//////    //        var request = new GetTranscriptionJobRequest
//////    //        {
//////    //            TranscriptionJobName = jobName
//////    //        };

//////    //        var response = await _transcribeService.GetTranscriptionJobAsync(request);
//////    //        var transcriptionJob = response.TranscriptionJob;

//////    //        if (transcriptionJob == null)
//////    //        {
//////    //            return NotFound("Transcription job not found.");
//////    //        }

//////    //        // Check the job status
//////    //        if (transcriptionJob.TranscriptionJobStatus == TranscriptionJobStatus.COMPLETED)
//////    //        {
//////    //            // Read the transcript file URI
//////    //            var transcriptFileUri = transcriptionJob.Transcript.TranscriptFileUri;

//////    //            // Fetch the transcript content
//////    //            using (HttpClient client = new HttpClient())
//////    //            {
//////    //                var transcriptResponse = await client.GetStringAsync(transcriptFileUri);
//////    //                var json = JObject.Parse(transcriptResponse);

//////    //                // Extract the transcript text
//////    //                var transcriptionText = json["results"]["transcripts"][0]["transcript"].ToString();

//////    //                return Ok(transcriptionText);
//////    //            }
//////    //        }
//////    //        else if (transcriptionJob.TranscriptionJobStatus == TranscriptionJobStatus.FAILED)
//////    //        {
//////    //            return BadRequest($"Transcription job failed: {transcriptionJob.FailureReason}");
//////    //        }

//////    //        return Ok("Transcription job is still in progress.");
//////    //    }
//////    //    catch (AmazonTranscribeServiceException transcribeEx)
//////    //    {
//////    //        return StatusCode((int)transcribeEx.StatusCode, $"Transcribe error: {transcribeEx.Message}");
//////    //    }
//////    //    catch (Exception ex)
//////    //    {
//////    //        return StatusCode(500, $"Internal server error: {ex.Message}");
//////    //    }
//////    //    //catch (AmazonS3Exception s3Ex)
//////    //    //{
//////    //    //    return StatusCode((int)s3Ex.StatusCode, $"S3 error: {s3Ex.Message}");
//////    //    //}

//////    //}

//////    //[HttpGet("transcription-results/{jobName}")]
//////    //public async Task<IActionResult> GetTranscriptionResults(string jobName)
//////    //{
//////    //    try
//////    //    {
//////    //        var request = new GetTranscriptionJobRequest
//////    //        {
//////    //            TranscriptionJobName = jobName
//////    //        };

//////    //        var response = await _transcribeService.GetTranscriptionJobAsync(request);
//////    //        var transcriptionJob = response.TranscriptionJob;

//////    //        if (transcriptionJob == null)
//////    //        {
//////    //            return NotFound("Transcription job not found.");
//////    //        }

//////    //        // Check the job status
//////    //        if (transcriptionJob.TranscriptionJobStatus == TranscriptionJobStatus.COMPLETED)
//////    //        {
//////    //            // Read the transcript file URI
//////    //            var transcriptFileUri = transcriptionJob.Transcript.TranscriptFileUri;

//////    //            // Fetch the transcript content
//////    //            using (HttpClient client = new HttpClient())
//////    //            {
//////    //                var transcriptResponse = await client.GetStringAsync(transcriptFileUri);
//////    //                var json = JObject.Parse(transcriptResponse);

//////    //                // Extract the transcript text
//////    //                var transcriptionText = json["results"]["transcripts"][0]["transcript"].ToString();

//////    //                return Ok(transcriptionText);
//////    //            }
//////    //        }
//////    //        else if (transcriptionJob.TranscriptionJobStatus == TranscriptionJobStatus.FAILED)
//////    //        {
//////    //            return BadRequest($"Transcription job failed: {transcriptionJob.FailureReason}");
//////    //        }

//////    //        return Ok("Transcription job is still in progress.");
//////    //    }
//////    //    catch (AmazonTranscribeServiceException transcribeEx)
//////    //    {
//////    //        return StatusCode((int)transcribeEx.StatusCode, $"Transcribe error: {transcribeEx.Message}");
//////    //    }
//////    //    catch (Exception ex)
//////    //    {
//////    //        return StatusCode(500, $"Internal server error: {ex.Message}");
//////    //    }
//////    //}

//////}
////using Microsoft.AspNetCore.Http;
////using Microsoft.AspNetCore.Mvc;
////using Amazon.S3;
////using Amazon.S3.Model;
////using Amazon.TranscribeService;
////using Amazon.TranscribeService.Model;
////using System.Threading.Tasks;
////using System.Net;
////using System.IO;

////[ApiController]
////[Route("upload")]
////public class UploadController : ControllerBase
////{
////    private readonly IAmazonS3 _s3Client;
////    private readonly IAmazonTranscribeService _transcribeService;

////    public UploadController(IAmazonS3 s3Client, IAmazonTranscribeService transcribeService)
////    {
////        _s3Client = s3Client;
////        _transcribeService = transcribeService;
////    }

////    [HttpPost("upload-file")]
////    public async Task<IActionResult> UploadFile(IFormFile file)
////    {
////        if (file == null || file.Length == 0)
////        {
////            return BadRequest("No file uploaded.");
////        }

////        try
////        {
////            var request = new PutObjectRequest
////            {
////                BucketName = "adminpermission",
////                Key = file.FileName,
////                InputStream = file.OpenReadStream(),
////                ContentType = file.ContentType
////            };

////            await _s3Client.PutObjectAsync(request);

////            var transcriptionJobName = file.FileName + "-transcription";
////            var transcriptionRequest = new StartTranscriptionJobRequest
////            {
////                TranscriptionJobName = transcriptionJobName,
////                LanguageCode = "he-IL",
////                Media = new Media
////                {
////                    MediaFileUri = $"https://s3.eu-north-1.amazonaws.com/adminpermission/{file.FileName}"
////                },
////                OutputBucketName = "adminpermission"
////            };

////            await _transcribeService.StartTranscriptionJobAsync(transcriptionRequest);

////            return Ok("File uploaded and transcription started successfully.");
////        }
////        catch (AmazonS3Exception s3Ex)
////        {
////            return StatusCode((int)s3Ex.StatusCode, $"S3 error: {s3Ex.Message}");
////        }
////        catch (AmazonTranscribeServiceException transcribeEx)
////        {
////            return StatusCode((int)transcribeEx.StatusCode, $"Transcribe error: {transcribeEx.Message}");
////        }
////    }

////    [HttpGet("transcript")]
////    public async Task<IActionResult> GetTranscriptAsync([FromQuery] string fileName)
////    {
////        var bucketName = "adminpermission";

////        try
////        {
////            var response = await _s3Client.GetObjectAsync(bucketName, fileName);
////            using (var reader = new StreamReader(response.ResponseStream))
////            {
////                var content = await reader.ReadToEndAsync();
////                return Content(content, "application/json");
////            }
////        }
////        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
////        {
////            return StatusCode(403, "Access Denied: Check permissions.");
////        }
////    }
////}
////using Amazon.S3;
////using Amazon.S3.Model;
////using Amazon.TranscribeService;
////using Amazon.TranscribeService.Model;
////using Microsoft.AspNetCore.Mvc;
////using Microsoft.Extensions.Configuration;
////using System;
////using System.IO;
////using System.Net;
////using System.Threading.Tasks;

////[ApiController]
////[Route("upload")]
////public class UploadController : ControllerBase
////{
////    private readonly IAmazonS3 _s3Client;
////    private readonly IAmazonTranscribeService _transcribeService;
////    private readonly string _bucketName;

////    public UploadController(IConfiguration config)
////    {
////        var awsAccessKey = config["AWS_ACCESS_KEY"];
////        var awsSecretKey = config["AWS_SECRET_KEY"];
////        var region = Amazon.RegionEndpoint.GetBySystemName(config["AWS_REGION"]);
////        _bucketName = config["BUCKET_NAME"];

////        var credentials = new Amazon.Runtime.BasicAWSCredentials(awsAccessKey, awsSecretKey);
////        _s3Client = new AmazonS3Client(credentials, region);
////        _transcribeService = new AmazonTranscribeServiceClient(credentials, region);
////    }
////    [HttpPost("upload-file")]
////    public async Task<IActionResult> UploadFile(IFormFile file)
////    {
////        if (file == null || file.Length == 0)
////        {
////            return BadRequest("No file uploaded.");
////        }

////        try
////        {
////            var request = new PutObjectRequest
////            {
////                BucketName = "adminpermission",
////                Key = file.FileName,
////                InputStream = file.OpenReadStream(),
////                ContentType = file.ContentType
////            };

////            await _s3Client.PutObjectAsync(request);

////            var transcriptionJobName = file.FileName + "-transcription";
////            var transcriptionRequest = new StartTranscriptionJobRequest
////            {
////                TranscriptionJobName = transcriptionJobName,
////                LanguageCode = "he-IL",
////                Media = new Media
////                {
////                    MediaFileUri = $"https://s3.eu-north-1.amazonaws.com/adminpermission/{file.FileName}"
////                },
////                OutputBucketName = "adminpermission"
////            };

////            await _transcribeService.StartTranscriptionJobAsync(transcriptionRequest);

////            return Ok("File uploaded and transcription started successfully.");
////        }
////        catch (AmazonS3Exception s3Ex)
////        {
////            return StatusCode((int)s3Ex.StatusCode, $"S3 error: {s3Ex.Message}");
////        }
////        catch (AmazonTranscribeServiceException transcribeEx)
////        {
////            return StatusCode((int)transcribeEx.StatusCode, $"Transcribe error: {transcribeEx.Message}");
////        }
////    }

////    //[HttpPost("upload-file")]
////    //public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
////    //{
////    //    if (file == null || file.Length == 0)
////    //        return BadRequest("No file uploaded.");

////    //    try
////    //    {
////    //        // שלב 1 - העלאה ל-S3
////    //        var uploadRequest = new PutObjectRequest
////    //        {
////    //            BucketName = _bucketName,
////    //            Key = file.FileName,
////    //            InputStream = file.OpenReadStream(),
////    //            ContentType = file.ContentType
////    //        };
////    //        await _s3Client.PutObjectAsync(uploadRequest);

////    //        // שלב 2 - התחלת תמלול
////    //        var transcriptionJobName = Guid.NewGuid().ToString(); // מומלץ שם ייחודי
////    //        var transcriptionRequest = new StartTranscriptionJobRequest
////    //        {
////    //            TranscriptionJobName = transcriptionJobName,
////    //            LanguageCode = "he-IL",
////    //            Media = new Media
////    //            {
////    //                MediaFileUri = $"s3://{_bucketName}/{file.FileName}"
////    //            },
////    //            OutputBucketName = _bucketName
////    //        };
////    //        await _transcribeService.StartTranscriptionJobAsync(transcriptionRequest);

////    //        return Ok($"File uploaded and transcription started. Job: {transcriptionJobName}");
////    //    }
////    //    catch (AmazonS3Exception s3Ex)
////    //    {
////    //        return StatusCode((int)s3Ex.StatusCode, $"S3 error: {s3Ex.Message}");
////    //    }
////    //    catch (AmazonTranscribeServiceException transcribeEx)
////    //    {
////    //        return StatusCode((int)transcribeEx.StatusCode, $"Transcribe error: {transcribeEx.Message}");
////    //    }
////    //    catch (Exception ex)
////    //    {
////    //        return StatusCode(500, $"Unexpected error: {ex.Message}");
////    //    }
////    //}

////    [HttpGet("transcript")]
////    public async Task<IActionResult> GetTranscriptAsync([FromQuery] string fileName)
////    {
////        try
////        {
////            // נשתמש ב Signed URL כדי להחזיר גישה מאובטחת לקובץ התמלול
////            var request = new GetPreSignedUrlRequest
////            {
////                BucketName = _bucketName,
////                Key = fileName,
////                Expires = DateTime.UtcNow.AddMinutes(15), // זמני
////                Verb = HttpVerb.GET
////            };

////            var url = _s3Client.GetPreSignedURL(request);
////            return Ok(new { Url = url });
////        }
////        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
////        {
////            return StatusCode(403, "Access Denied: Check permissions.");
////        }
////        catch (Exception ex)
////        {
////            return StatusCode(500, $"Unexpected error: {ex.Message}");
////        }
////    }
////}
//using Amazon.S3;
//using Amazon.S3.Model;
//using Amazon.TranscribeService;
//using Amazon.TranscribeService.Model;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Threading.Tasks;

//[ApiController]
//[Route("upload")]
//public class UploadController : ControllerBase
//{
//    private readonly IAmazonS3 _s3Client;
//    private readonly IAmazonTranscribeService _transcribeService;
//    private readonly string _bucketName;

//    public UploadController(IConfiguration config)
//    {
//        var awsAccessKey = config["AWS_ACCESS_KEY"];
//        var awsSecretKey = config["AWS_SECRET_KEY"];
//        var region = Amazon.RegionEndpoint.GetBySystemName(config["AWS_REGION"]);
//        _bucketName = config["BUCKET_NAME"];

//        var credentials = new Amazon.Runtime.BasicAWSCredentials(awsAccessKey, awsSecretKey);
//        _s3Client = new AmazonS3Client(credentials, region);
//        _transcribeService = new AmazonTranscribeServiceClient(credentials, region);
//    }

//    [HttpPost("upload-file")]
//    public async Task<IActionResult> UploadFile(IFormFile file)
//    {
//        if (file == null || file.Length == 0)
//        {
//            return BadRequest("No file uploaded.");
//        }

//        try
//        {
//            var request = new PutObjectRequest
//            {
//                BucketName = _bucketName,
//                Key = file.FileName,
//                InputStream = file.OpenReadStream(),
//                ContentType = file.ContentType
//            };

//            await _s3Client.PutObjectAsync(request);

//            var transcriptionJobName = file.FileName + "-transcription";
//            var transcriptionRequest = new StartTranscriptionJobRequest
//            {
//                TranscriptionJobName = transcriptionJobName,
//                LanguageCode = "he-IL",
//                Media = new Media
//                {
//                    MediaFileUri = $"https://s3.{_s3Client.Config.RegionEndpoint.SystemName}.amazonaws.com/{_bucketName}/{file.FileName}"
//                },
//                OutputBucketName = _bucketName
//            };

//            await _transcribeService.StartTranscriptionJobAsync(transcriptionRequest);

//            return Ok("File uploaded and transcription started successfully.");
//        }
//        catch (AmazonS3Exception s3Ex)
//        {
//            return StatusCode((int)s3Ex.StatusCode, $"S3 error: {s3Ex.Message}");
//        }
//        catch (AmazonTranscribeServiceException transcribeEx)
//        {
//            return StatusCode((int)transcribeEx.StatusCode, $"Transcribe error: {transcribeEx.Message}");
//        }
//    }

//    [HttpGet("transcript")]
//    public async Task<IActionResult> GetTranscriptAsync([FromQuery] string fileName)
//    {
//        try
//        {
//            var request = new GetPreSignedUrlRequest
//            {
//                BucketName = _bucketName,
//                Key = fileName,
//                Expires = DateTime.UtcNow.AddMinutes(15),
//                Verb = HttpVerb.GET
//            };

//            var url = _s3Client.GetPreSignedURL(request);
//            return Ok(new { Url = url });
//        }
//        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
//        {
//            return StatusCode(403, "Access Denied: Check permissions.");
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, $"Unexpected error: {ex.Message}");
//        }
//    }

//    // פונקציה פרטית לעדכון Content-Type של הקובץ ל-application/json
//    private async Task SetJsonContentTypeAsync(string key)
//    {
//        var copyRequest = new CopyObjectRequest
//        {
//            SourceBucket = _bucketName,
//            SourceKey = key,
//            DestinationBucket = _bucketName,
//            DestinationKey = key,
//            MetadataDirective = S3MetadataDirective.REPLACE,
//            ContentType = "application/json"
//        };

//        await _s3Client.CopyObjectAsync(copyRequest);
//    }

//    // נקודת קצה לעדכון Content-Type של קובץ JSON בתמלול
//    [HttpPost("set-json-content-type")]
//    public async Task<IActionResult> SetContentType([FromQuery] string fileName)
//    {
//        try
//        {
//            await SetJsonContentTypeAsync(fileName);
//            return Ok("Content-Type updated to application/json.");
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, $"Error updating Content-Type: {ex.Message}");
//        }
//    }
//}
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.TranscribeService;
using Amazon.TranscribeService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
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

    [HttpGet("transcript")]
    public async Task<IActionResult> GetAndSetTranscriptAsync([FromQuery] string fileName)
    {
        try
        {
            // בודקים אם הקובץ קיים לפני העדכון
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

            // עדכון Content-Type ל-application/json
            var copyRequest = new CopyObjectRequest
            {
                SourceBucket = _bucketName,
                SourceKey = fileName,
                DestinationBucket = _bucketName,
                DestinationKey = fileName,
                MetadataDirective = S3MetadataDirective.REPLACE,
                ContentType = "application/json"
            };

            await _s3Client.CopyObjectAsync(copyRequest);

            // יצירת URL חתום
            var urlRequest = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = fileName,
                Expires = DateTime.UtcNow.AddMinutes(15),
                Verb = HttpVerb.GET
            };

            var url = _s3Client.GetPreSignedURL(urlRequest);

            return Ok(new { Url = url });
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
