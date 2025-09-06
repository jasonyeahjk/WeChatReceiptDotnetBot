using System.ComponentModel.DataAnnotations;

namespace DonutPaymentService.API.Application.DTOs
{
    public class RecognitionRequestDto
    {
        [Required(ErrorMessage = "UserId is required.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "ImageBase64 is required.")]
        public string ImageBase64 { get; set; }
    }
}

