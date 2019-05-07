namespace Framework.Logic.Tasks.Config.Scheduled
{
    using System.Configuration;

    /// <summary>
    ///     Provides a configuration collection of named queues.
    /// </summary>
    [ConfigurationCollection(typeof(ScheduledTaskElement))]
    public class ScheduledTaskCollection : ConfigurationElementCollection
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
        public ScheduledTaskElement this[int index]
        {
            get => (ScheduledTaskElement)this.BaseGet(index);
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
        public new ScheduledTaskElement this[string name] => (ScheduledTaskElement)this.BaseGet(name);

        public int IndexOf(ScheduledTaskElement queue)
        {
            return this.BaseIndexOf(queue);
        }

        public void RemoveAt(int index)
        {
            this.BaseRemoveAt(index);
        }

        public void Add(ScheduledTaskElement item)
        {
            this.BaseAdd(item);
        }

        public void Clear()
        {
            this.BaseClear();
        }

        public bool Contains(ScheduledTaskElement item)
        {
            return this.BaseIndexOf(item) >= 0;
        }

        public void CopyTo(ScheduledTaskElement[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
        }

        public bool Remove(ScheduledTaskElement item)
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
            return new ScheduledTaskElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var queue = (ScheduledTaskElement)element;
            return this.GetKey(queue);
        }

        /// <summary>
        ///     Gets the key by which named queue elements are mapped in the base class.
        /// </summary>
        /// <param name="queue">The named queue element to get the key from.</param>
        /// <returns>The key.</returns>
        private string GetKey(ScheduledTaskElement queue)
        {
            return queue.Name;
        }
    }
}