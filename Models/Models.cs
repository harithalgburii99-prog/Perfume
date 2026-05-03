using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PerfumeStore.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        [StringLength(100)]
        public string Username { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد غير صحيحة")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [StringLength(256)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [StringLength(50)]
        public string Role { get; set; } = "User";
    }

    public class Perfume
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم العطر مطلوب")]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [Range(0.01, 99999.99, ErrorMessage = "السعر يجب أن يكون أكبر من صفر")]
        public decimal? Price { get; set; }

        [StringLength(500)]
        public string ImagePath { get; set; }
    }

    public class Cart
    {
        public int CartId { get; set; }
        
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public int PerfumeId { get; set; }
        [ForeignKey("PerfumeId")]
        public Perfume Perfume { get; set; }

        public System.DateTime OrderDate { get; set; } = System.DateTime.Now;
    }

    public class Order
    {
        public int OrderId { get; set; }

        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public int? PerfumeId { get; set; }
        [ForeignKey("PerfumeId")]
        public Perfume Perfume { get; set; }

        public System.DateTime? OrderDate { get; set; } = System.DateTime.Now;
    }

    public class CartViewModel
    {
        public int CartId { get; set; }
        public string PerfumeName { get; set; }
        public decimal Price { get; set; }
    }
}
