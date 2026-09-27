using c__service_2026.Dto;
using c__service_2026.Interfaces;
using c__service_2026.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;

namespace FaceGallery.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecognizeController : ControllerBase
    {
        private const long MaxFileSizeBytes =
            15 * 1024 * 1024;

        private readonly IRecognitionService
            _recognitionService;

        public RecognizeController(
            IRecognitionService recognitionService
        )
        {
            _recognitionService =
                recognitionService;
        }

        [HttpPost]
        [RequestSizeLimit(MaxFileSizeBytes)]
        public async Task<ActionResult<List<MatchResultDto>>> Recognize(
            [FromQuery] int galleryId,
            IFormFile file,
            [FromQuery] double? threshold = null
        )
        {
            if (
                file == null ||
                file.Length == 0
            )
            {
                return BadRequest(
                    "Upload a reference photo in the 'file' field."
                );
            }

            try
            {
                using var stream =
                    file.OpenReadStream();

                var results =
                    await _recognitionService
                        .FindMatchesAsync(
                            stream,
                            galleryId,
                            threshold
                        );

                return Ok(results);
            }
            catch (NoFaceDetectedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                return StatusCode(
                    503,
                    ex.Message
                );
            }
        }

        [HttpPost("by-user")]
        [RequestSizeLimit(MaxFileSizeBytes)]
        public async Task<ActionResult<List<MatchResultDto>>> RecognizeForUser(
            [FromQuery] int userId,
            IFormFile file,
            [FromQuery] double? threshold = null
        )
        {
            if (
                file == null ||
                file.Length == 0
            )
            {
                return BadRequest(
                    "Upload a reference photo in the 'file' field."
                );
            }

            try
            {
                using var stream =
                    file.OpenReadStream();

                var results =
                    await _recognitionService
                        .FindMatchesForUserAsync(
                            stream,
                            userId,
                            threshold
                        );

                return Ok(results);
            }
            catch (NoFaceDetectedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                return StatusCode(
                    503,
                    ex.Message
                );
            }
        }
    }
}