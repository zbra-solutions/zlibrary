﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZLibrary.Web.Converters
{
    public interface IConverter<TModel, TViewItem> : IToModelConverter<TModel, TViewItem>, IFromModelConverter<TModel, TViewItem>
    {
    }
}
