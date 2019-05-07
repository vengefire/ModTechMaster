namespace Framework.Domain.Queue
{
    using System;

    public enum MessageProcessingMode
    {
        LocalChildNodeConcurrent
    }

    public static class Convert
    {
        public static MessageProcessingMode ToMessageProcessingMode(object value)
        {
            return Convert.ToMessageProcessingMode(value.ToString());
        }

        public static MessageProcessingMode ToMessageProcessingMode(string value)
        {
            if (value == "LocalChildNodeConcurrent")
            {
                return MessageProcessingMode.LocalChildNodeConcurrent;
            }

            throw new InvalidProgramException(
                                              string.Format(
                                                            "Invalid value [{0}] encountered converting to MessageProcessingMode, expected [{1}].",
                                                            value,
                                                            "Single|SingleConcurrent|Dispatch"));
        }
    }
}