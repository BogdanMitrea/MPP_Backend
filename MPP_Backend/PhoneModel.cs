using System.ComponentModel.DataAnnotations;

namespace MPP_BackEnd
{
    public class PhoneModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public string Producer { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public int Memory { get; set; }
        public string Photo { get; set; }

        public int Store { get; set; }
    }
}
