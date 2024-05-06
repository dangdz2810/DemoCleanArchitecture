using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DemoCleanArchitecture.Domain.Entities
{
    public class ForgotPasswordRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Code { get; set; }
        public DateTime? ExpirationTime { get; set; }

        public string? UserName { get; set; }
    }
}
