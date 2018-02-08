using System;
using System.Collections.Generic;

namespace Tesseract.Common.ComposerImposter
{
    public class ComponentContext : IComponentContext
    {
        public T GetComponent<T>()
        {
            throw new NotImplementedException();
        }

        public Type GetComponent(Type t)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAllComponents<T>()
        {
            throw new NotImplementedException();
        }

        public void ProcessApplicationConfiguration()
        {
            throw new NotImplementedException();
        }
    }
}