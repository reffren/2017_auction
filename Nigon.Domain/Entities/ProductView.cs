using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nigon.Domain.Entities
{
    public class ProductView
    {
        [Key]
        public int ProductViewID { get; set; }
        public int RateID { get; set; }

        [Display(Name = "Стартовая цена")]
        [Required(ErrorMessage = "Пожалуйста заполните поле: 'Стартовая цена'")]
        [Range(typeof(decimal), "5,0", "9999999", ErrorMessage = "Наименьшая цена - 5 рублей, в качестве разделителя дробной и целой части используется запятая")]
        public decimal StartPrice { get; set; }

        [Display(Name = "Шаг ставки")]
        [Required(ErrorMessage = "Пожалуйста заполните поле: 'Шаг ставки'")]
        [Range(typeof(decimal), "5,0", "9999999", ErrorMessage = "Наименьшая цена - 5 рублей, в качестве разделителя дробной и целой части используется запятая")]
        public decimal StepPrice { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Осталось до конца торгов")]
        [Required(ErrorMessage = "Пожалуйста заполните поле: 'Осталось до конца торгов'")]
        public DateTime TimeOfAuctionEnd { get; set; }

        [Display(Name = "Состояние товара")]
        [Required(ErrorMessage = "Пожалуйста заполните поле: 'Состояние товара'")]
        public string StateOfProduct { get; set; }

        [Display(Name = "Начало торгов")]
        [DataType(DataType.DateTime)]
     //   [DateValidation(ErrorMessage = "Please enter a valid date in the format dd/mm/yyyy hh:mm")]
        public DateTime StartOfAuction { get; set; }

        [Display(Name = "Завершение торгов")]
        [DataType(DataType.DateTime)]
        public DateTime EndOfAuction { get; set; }

        [Display(Name = "Местоположение")]
        [Required(ErrorMessage = "Пожалуйста заполните поле: 'Местоположение'")]
        public string Location { get; set; }

    }

    public class DateValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime dt;
            bool parsed = DateTime.TryParse((string)value, out dt);
            if (!parsed)
                return false;
            return true;
        }
    }
}