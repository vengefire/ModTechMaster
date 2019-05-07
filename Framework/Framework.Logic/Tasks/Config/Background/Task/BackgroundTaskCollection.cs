namespace Framework.Logic.Tasks.Config.Background.Task
{
    using System.Configuration;

    /// <summary>
    ///     Provides a configuration collection of named queues.
    /// </summary>
    [ConfigurationCollection(typeof(BackgroundTaskElement))]
    public class BackgroundTaskCollection : ConfigurationElementCollection
    {
        /// <summary>
        ///     Gets the number of named queue elements in this instance.
        /// </summary>
        public new int Count => base.Count;

        public new bool IsReadOnly => false;

        /// <summary>
        ///     Gets or sets the named queue element for the given index.
        /// </summary>
        /// <param name="index">The index of the named queue element to get or set.</param>
        /// <returns>The named queue element.</returns>
        public BackgroundTaskElement this[int index]
        {
            get => (BackgroundTaskElement)this.BaseGet(index);
            set
            {
                if (this.BaseGet(index) != null)
                {
                    this.BaseRemove(index);
                }

                this.BaseAdd(index, value);
            }
        }

        /// <summary>
        ///     Gets or sets the named queue element for the given name.
        /// </summary>
        /// <param name="name">The name of the named queue element to get or set.</param>
        /// <returns>The named queue element.</returns>
        public new BackgroundTaskElement this[string name] => (BackgroundTaskElement)this.BaseGet(name);

        public int IndexOf(BackgroundTaskElement queue)
        {
            return this.BaseIndexOf(queue);
        }

        public void RemoveAt(int index)
        {
            this.BaseRemoveAt(index);
        }

        public void Add(BackgroundTaskElement item)
        {
            this.BaseAdd(item);
        }

        public void Clear()
        {
            this.BaseClear();
        }

        public bool Contains(BackgroundTaskElement item)
        {
            return this.BaseIndexOf(item) >= 0;
        }

        public void CopyTo(BackgroundTaskElement[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
        }

        public bool Remove(BackgroundTaskElement item)
        {
            if (this.BaseIndexOf(item) >= 0)
            {
                this.BaseRemove(item);
                return true;
            }

            return false;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new BackgroundTaskElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var queue = (BackgroundTaskElement)element;
            return this.GetKey(queue);
        }

        /// <summary>
        ///     Gets the key by which named queue elements are mapped in the base class.
        /// </summary>
        /// <param name="queue">The named queue element to get the key from.</param>
        /// <returns>The key.</returns>
        private string GetKey(BackgroundTaskElement queue)
        {
            return queue.Name;
        }
    }
}