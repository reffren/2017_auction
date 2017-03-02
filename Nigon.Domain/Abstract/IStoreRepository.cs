﻿using Nigon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nigon.Domain.Abstract
{
    public interface IStoreRepository
    {
        IQueryable<Category> Categories { get; }
    }
}
