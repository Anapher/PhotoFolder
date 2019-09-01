using System;

namespace PhotoFolder.Wpf.Services
{
    public class PhotoFolderSynchronizationEvent
    {
        public event EventHandler PhotoFolderSynchronized;

        public void OnPhotoFolderSynchronized()
        {
            PhotoFolderSynchronized?.Invoke(this, EventArgs.Empty);
        }
    }
}
