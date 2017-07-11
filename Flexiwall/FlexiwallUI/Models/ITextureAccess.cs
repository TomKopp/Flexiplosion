using System;

namespace FlexiWallUI.Models
{
    public interface ITextureAccess<T> where T: class
    {
        T Load(String connectionString);

        void Save(String connectionString, T repository);
    }
}
