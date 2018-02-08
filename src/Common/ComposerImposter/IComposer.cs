using System;
using System.Collections.Generic;

namespace Tesseract.Common.ComposerImposter
{
    public interface IComposer
    {
        T GetComponent<T>();
        Type GetComponent(Type t);
        IEnumerable<T> GetAllComponents<T>();
    }
}