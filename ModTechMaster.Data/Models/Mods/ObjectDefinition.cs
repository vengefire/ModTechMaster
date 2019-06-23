namespace ModTechMaster.Data.Models.Mods
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;

    using Newtonsoft.Json.Linq;

    public class ObjectDefinition : JsonObjectSourcedFromFile, IObjectDefinition, INotifyPropertyChanged
    {
        private List<IObjectReference<IReferenceableObject>> objectReferences;

        public ObjectDefinition(
            ObjectType objectType,
            IObjectDefinitionDescription objectDescription,
            dynamic jsonObject,
            string filePath,
            IReferenceFinderService referenceFinderService)
            : base(objectType, filePath, (JObject)jsonObject)
        {
            this.ReferenceFinderService = referenceFinderService;
            this.ObjectDescription = objectDescription;
            this.MetaData = new Dictionary<string, dynamic>();
            this.Tags = new Dictionary<string, List<string>>();
            this.DependencyRelationships = referenceFinderService.GetDependencyRelationships(this.ObjectType);
            this.DependentRelationships = referenceFinderService.GetDependentRelationships(this.ObjectType);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<IObjectReference<IReferenceableObject>> Dependencies =>
            this.ObjectReferences.Where(reference => reference.ObjectReferenceType == ObjectReferenceType.Dependency)
                .ToList();

        public List<IObjectRelationship> DependencyRelationships { get; }

        public List<IObjectRelationship> DependentRelationships { get; }

        public List<IObjectReference<IReferenceableObject>> Dependents =>
            this.ObjectReferences.Where(reference => reference.ObjectReferenceType == ObjectReferenceType.Dependent)
                .ToList();

        public string HumanReadableText => this.JsonString;

        public override string Id => this.MetaData.ContainsKey(Keywords.Id) ? this.MetaData[Keywords.Id] : this.Name;

        public Dictionary<string, dynamic> MetaData { get; }

        public override string Name =>
            this.MetaData.ContainsKey(Keywords.Name) ? this.MetaData[Keywords.Name] : this.SourceFileName;

        public IObjectDefinitionDescription ObjectDescription { get; }

        public List<IObjectReference<IReferenceableObject>> ObjectReferences
        {
            get
            {
                if (this.objectReferences == null)
                {
                    this.objectReferences = this.ReferenceFinderService.GetObjectReferences(this);
                    this.objectReferences.Sort(
                        (reference, objectReference) => 
                            reference.ReferenceObject == null ? 1 : 
                            objectReference.ReferenceObject == null ? -1 : 
                            string.CompareOrdinal(reference.ReferenceObject.ObjectType.ToString(), objectReference?.ReferenceObject?.ObjectType.ToString()));

                    this.OnPropertyChanged();
                    if (this.Dependencies.Any())
                    {
                        this.OnPropertyChanged(nameof(this.Dependencies));
                    }

                    if (this.Dependents.Any())
                    {
                        this.OnPropertyChanged(nameof(this.Dependents));
                    }
                }

                return this.objectReferences;
            }
        }

        public ObjectStatus ObjectStatus
        {
            get => ObjectStatus.Nominal;
            set => this.ObjectStatus = value;
        }

        public IReferenceFinderService ReferenceFinderService { get; }

        public Dictionary<string, List<string>> Tags { get; }

        public virtual void AddMetaData()
        {
            if (this.ObjectDescription?.Id != null)
            {
                if (!this.MetaData.ContainsKey(Keywords.Id))
                {
                    this.MetaData.Add(Keywords.Id, this.ObjectDescription.Id);
                }
                else
                {
                    this.MetaData[Keywords.Id] = this.ObjectDescription.Id;
                }
            }
            else if (this.JsonObject?.Id != null)
            {
                this.MetaData.Add(Keywords.Id, this.JsonObject.Id);
            }
            else if (this.JsonObject?.identifier != null)
            {
                this.MetaData.Add(Keywords.Id, this.JsonObject.identifier);
            }
            else if (this.JsonObject?.ID != null)
            {
                this.MetaData.Add(Keywords.Id, this.JsonObject.ID);
            }

            if (this.ObjectDescription?.Name != null)
            {
                this.MetaData.Add(Keywords.Name, this.ObjectDescription.Name);
            }
            else if (this.JsonObject?.Name != null)
            {
                this.MetaData.Add(Keywords.Name, this.JsonObject.Name);
            }

            // If we didn't find a Name in our json store (which may not be there if we're a resource object) then add our default name (FileName)
            if (!this.MetaData.ContainsKey(Keywords.Name))
            {
                this.MetaData.Add(Keywords.Name, this.Name);
            }

            // Similarly if we didnt find an ID, use the name as ID.
            if (!this.MetaData.ContainsKey(Keywords.Id))
            {
                this.MetaData.Add(Keywords.Id, this.Name);
            }

            // Add tag data. Not all relationships are defined via tight IDs. Some are defined by loose tags.
            var jobject = this.JsonObject as JObject;
            var tags = jobject.Properties().FirstOrDefault(property => property.Name.Contains("Tags"))?.Value?.First;
            var tagList = new List<string>();
            if (tags != null)
            {
                foreach (var tagArray in tags.Children())
                {
                    foreach (var tag in tagArray)
                    {
                        tagList.Add(tag.ToString());
                    }
                }
            }

            this.Tags.Add(Keywords.MyTags, tagList);
        }

        public override IValidationResult ValidateObject()
        {
            // Check each dependency is satisfied...
            var unsatisfiedDependencies = this.Dependencies.Where(reference => !reference.IsValid).ToList();
            if (unsatisfiedDependencies.Any())
            {
                return new ValidationResult
                           {
                               Result = ValidationResultEnum.Failure,
                               ValidationResultReasons = unsatisfiedDependencies.Select(
                                       reference => new ValidationResultReason(
                                           this,
                                           $"This [{this.ObjectType}]-[{this.Name}] failed to satisfy dependency of [{reference.Relationship.DependencyType}]-[{reference.ReferenceKey}] via [{reference.Relationship.DependentKey}]."))
                                   .Cast<IValidationResultReason>().ToList()
                           };
            }

            return ValidationResult.SuccessValidationResult();
        }

        // [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}