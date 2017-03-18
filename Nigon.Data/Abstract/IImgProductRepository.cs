using Nigon.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nigon.Data.Abstract
{
    public interface IImgProductRepository
    {
        IQueryable<ImgProduct> ImgProducts { get; }
        void SaveImage(ImgProduct imgProduct);
        void DeleteImgProduct(ImgProduct imgProduct);
    }
}
