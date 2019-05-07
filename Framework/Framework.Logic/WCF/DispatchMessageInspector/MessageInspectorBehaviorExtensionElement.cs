namespace Framework.Logic.WCF.DispatchMessageInspector
{
    using System;
    using System.ServiceModel.Configuration;

    public class MessageInspectorBehaviorExtensionElement : BehaviorExtensionElement
    {
        public override Type BehaviorType => typeof(MessageInspectorBehavior);

        protected override object CreateBehavior()
        {
            return new MessageInspectorBehavior();
        }
    }
}