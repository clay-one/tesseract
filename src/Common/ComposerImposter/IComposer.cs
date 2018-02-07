using System;

namespace Tesseract.Common.ComposerImposter
{
    public interface IComposer
    {
        T GetComponent<T>();
        Type GetComponent(Type t);
    }
}