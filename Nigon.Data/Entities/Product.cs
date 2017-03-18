using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nigon.Data.Entities
{
    public class Product
    {
        public int ProductID { get; set; }

        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Пожалуйста заполните поле: 'Название'")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        [Required(ErrorMessage = "Пожалуйста заполните поле: 'Описание'")]
        public string Description { get; set; }

        [Display(Name = "Цена")]
        [Required(ErrorMessage = "Пожалуйста заполните поле: 'Текущая цена'")]
        [Range(typeof(decimal), "5,0", "9999999", ErrorMessage = "Наименьшая цена - 5 рублей, в качестве разделителя дробной и целой части используется запятая")]
        public decimal Price { get; set; }
        public string ImgPreview { get; set; }
        public int UserID { get; set; }
        public virtual User User { get; set; }
        public int ProductViewID { get; set; }
        public virtual ProductView ProductView { get; set; }
        public int SubCategoryID { get; set; }
        public virtual SubCategory SubCategory { get; set; }
    }
}