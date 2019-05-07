using System.Configuration;

namespace Framework.Logic.Tasks.Config.Background.Trigger
{
    /// <summary>
    ///     Provides a configuration collection of named queues.
    /// </summary>
    [ConfigurationCollection(typeof(TaskTriggerElement))]
    public class TaskTriggerCollection : ConfigurationElementCollection
    {
        /// <summary>
        ///     Gets the number of named queue elements in this instance.
        /// </summary>
        public new int Count
        {
            get { return base.Count; }
        }

        public new bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///     Gets or sets the named queue element for the given index.
        /// </summary>
        /// <param name="index">The index of the named queue element to get or set.</param>
        /// <returns>The named queue element.</returns>
        public TaskTriggerElement this[int index]
        {
            get { return (TaskTriggerElement) BaseGet(index); }

            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemove(index);
                }

                BaseAdd(index, value);
            }
        }

        /// <summary>
        ///     Gets or sets the named queue element for the given name.
        /// </summary>
        /// <param name="name">The name of the named queue element to get or set.</param>
        /// <returns>The named queue element.</returns>
        public new TaskTriggerElement this[string name]
        {
            get { return (TaskTriggerElement) BaseGet(name); }
        }

        public int IndexOf(TaskTriggerElement queue)
        {
            return BaseIndexOf(queue);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Add(TaskTriggerElement item)
        {
            BaseAdd(item);
        }

        public void Clear()
        {
            BaseClear();
        }

        public bool Contains(TaskTriggerElement item)
        {
            return BaseIndexOf(item) >= 0;
        }

        public void CopyTo(TaskTriggerElement[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
        }

        public bool Remove(TaskTriggerElement item)
        {
            if (BaseIndexOf(item) >= 0)
            {
                BaseRemove(item);
                return true;
            }

            return false;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new TaskTriggerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var queue = (TaskTriggerElement) element;
            return GetKey(queue);
        }

        /// <summary>
        ///     Gets the key by which named queue elements are mapped in the base class.
        /// </summary>
        /// <param name="queue">The named queue element to get the key from.</param>
        /// <returns>The key.</returns>
        private string GetKey(TaskTriggerElement queue)
        {
            return queue.Name;
        }
    }
}