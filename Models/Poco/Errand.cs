using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EnvironmentCrime.Models
{
    public class Errand
    {
        public int ErrandID { get; set; }

        [Display(Name = "Var har brottet skett någonstans?")]
        [Required(ErrorMessage = "Du måste ange en plats")]
        public string Place { get; set; }

        [Display(Name = "Vilken typ av brott?")]
        [Required(ErrorMessage = "Du måste ange en typ av brott")]
        public string TypeOfCrime { get; set; }

        [Display(Name = "När skedde brottet?")]
        [Required(ErrorMessage = "Du måste ange en tid")]
        [DataType(DataType.Date)]
        public DateTime DateOfObservation { get; set; }

        public string Observation { get; set; }

        public string InvestigatorInfo { get; set; }

        public string InvestigatorAction { get; set; }

        [Display(Name = "Ditt namn (för- och efternamn):")]
        [Required(ErrorMessage = "Du måste ange ett namn")]
        public string InformerName { get; set; }

        [Display(Name = "Din telefon:")]
        [Required(ErrorMessage = "Du måste ange ett telefonnummer")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Felaktigt format. Var god ange 10 siffror")] //telefonnumret måste vara 10 siffror
        public string InformerPhone { get; set; }

        public string StatusId { get; set; }

        public string DepartmentId { get; set; }

        public string EmployeeId { get; set; }
        public string RefNumber { get; set; }

        public ICollection<Sample> Samples { get; set; }
        public ICollection<Picture> Pictures { get; set; }
    }
}
