using Homework.Convertors;
using Homework.Models;
using Microsoft.AspNetCore.Mvc;

namespace Homework.Controllers
{
    [ApiController]
    [Route("api/documents")]
    public class DocumentController : ControllerBase
    {
        private readonly ILogger<DocumentController> _logger;
        private readonly IFileManager _fileManager;

        public DocumentController(
            ILogger<DocumentController> logger,
            IFileManager fileManager
        )
        {
            _logger = logger;
            _fileManager = fileManager;
        }

        [HttpPost]
        [Route("convert-by-api")]
        public async Task<ActionResult<Models.ApiDTO.Responses.ConvertByApiPayload>> ConvertByApi([FromBody] Models.ApiDTO.Requests.ConvertByApiPayload payload)
        {
            IFile outputFile;
            try
            {
                outputFile = await _fileManager.ConvertFileFromApi(
                    Convert.FromBase64String(payload.Base64Content),
                    await GetDocumentType(payload.DocumentType),
                    payload.InputFormat,
                    payload.OutputFormat
                );
            }
            catch (TypeLoadException)
            {
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Problem with file converting with message: '{ex.Message}'.");

                return new StatusCodeResult(500);
            }

            return Ok(new Models.ApiDTO.Responses.ConvertByApiPayload()
            {
                Base64Content = Convert.ToBase64String(outputFile.Content),
                OuputFormat = payload.OutputFormat
            });
        }

        [HttpPost]
        [Route("convert-on-storage")]
        public async Task<ActionResult> ConvertOnStorage([FromBody] Models.ApiDTO.Requests.ConvertOnStoragePayload payload)
        {
            try
            {
                await _fileManager.ConvertFileOnStorage(
                    payload.InputFilename,
                    payload.InputFileLocation,
                    await GetDocumentType(payload.DocumentType),
                    payload.InputFormat,
                    payload.OutputFilename,
                    payload.OutputFileLocation,
                    payload.OutputFormat
                );
            }
            catch (TypeLoadException)
            {
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Problem with file converting with message: '{ex.Message}'.");

                return new StatusCodeResult(500);
            }
            return Ok();
        }

        [HttpPost]
        [Route("convert-from-url")]
        public async Task<ActionResult<Models.ApiDTO.Responses.ConvertByApiPayload>> ConvertFromUrl([FromBody] Models.ApiDTO.Requests.ConvertFromUrlPayload payload)
        {
            IFile outputFile;
            try
            {
                outputFile = await _fileManager.ConvertFileFromUrl(
                    payload.Url,
                    await GetDocumentType(payload.DocumentType),
                    payload.InputFormat,
                    payload.OutputFormat
                );
            }
            catch (TypeLoadException)
            {
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Problem with file converting with message: '{ex.Message}'.");

                return new StatusCodeResult(500);
            }

            return Ok(new Models.ApiDTO.Responses.ConvertByApiPayload()
            {
                Base64Content = Convert.ToBase64String(outputFile.Content),
                OuputFormat = payload.OutputFormat
            });
        }

        private Task<Type> GetDocumentType(string documentType)
        {
            var detectedDocumentType = Type.GetType(documentType, true);

            if (detectedDocumentType == null)
                throw new ArgumentException();

            return Task.FromResult(detectedDocumentType);
        }
    }
}