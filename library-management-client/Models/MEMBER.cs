using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia_DependencyInjection.Models
{
    public class MEMBER
    {
        [Key]
        public int MemberId { get; set; }

        [MaxLength(12)]
        public string CitizenID { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Address { get; set; }

        [MaxLength(11)]
        public string PhoneNum { get; set; }

        public int Credit { get; set; }

        public int Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        [MaxLength(4)]
        public int EmployeeId { get; set; }
        public EMPLOYEE Employee { get; set; }

        public MEMBER(
            string citizenId,
            string name, 
            string address, 
            string phoneNum,
            string gender,
            DateTime dateOfBirth,
            int memberId=0)
        {
            MemberId = memberId;
            CitizenID = citizenId;
            Name = name;
            Address = address;
            PhoneNum = phoneNum;
            if (gender == "Male") Gender = 1;
            else Gender = 0;
            DateOfBirth = dateOfBirth;
        }
    }
}
