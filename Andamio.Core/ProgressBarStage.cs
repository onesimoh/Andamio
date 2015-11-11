using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Andamio;
using Andamio.Serialization;

namespace Andamio
{
    #region Status
    public enum ProgressBarStageStatus { Completed, Current, Pending, Failed }
    public static class ProgressBarStageStatusExtensions
    {
        public static bool IsCompleted(this ProgressBarStageStatus status)
        {
            return status == ProgressBarStageStatus.Completed;
        }
        public static bool IsCurrent(this ProgressBarStageStatus status)
        {
            return status == ProgressBarStageStatus.Current;
        }
        public static bool IsPending(this ProgressBarStageStatus status)
        {
            return status == ProgressBarStageStatus.Pending;
        }
        public static bool IsFailure(this ProgressBarStageStatus status)
        {
            return status == ProgressBarStageStatus.Failed;
        }
    }

    #endregion


    #region Attributes
    [Flags]
    public enum ProgressBarStageAttributes 
    { 
        None = 0x01, 
        Optional = 0x02,
        Hidden = 0x04,
    }

    public static class ProgressBarStageAttributesExtensions
    {
        public static bool IsOptional(this ProgressBarStageAttributes attributes)
        {
            return (attributes & ProgressBarStageAttributes.Optional) == ProgressBarStageAttributes.Optional;
        }
        public static bool IsHidden(this ProgressBarStageAttributes attributes)
        {
            return (attributes & ProgressBarStageAttributes.Hidden) == ProgressBarStageAttributes.Hidden;
        }
    }

    #endregion


    #region Collection
    [DebuggerDisplay("Count = {Count}")]
    public sealed class ProgressBarStages : IEnumerable<ProgressBarStage>, ICollection<ProgressBarStage>
    {
        #region Constructors
        public ProgressBarStages() : base()
        {
        }

        public ProgressBarStages(IEnumerable<ProgressBarStage> stages)
        {
            AddRange(stages);
        }
        
        public ProgressBarStages(params ProgressBarStage[] stages)
        {
            AddRange(stages);
        }
        
        public ProgressBarStages(params Enum[] enumValues) : base()
        {
            if (enumValues != null && enumValues.Any())
            {
                enumValues.ForEach(enumValue => Add(new ProgressBarStage(enumValue)));
            }
        }

        #endregion

        #region Nodes
        public LinkedListNode<ProgressBarStage> GetNode(string keyName)
        {
            var stageBarNode = _stages.First;
            while (stageBarNode != null)
            {
                ProgressBarStage stage = stageBarNode.Value;
                if (stage.KeyName.Equals(keyName, StringComparison.OrdinalIgnoreCase))
                {
                    return stageBarNode;
                }
                stageBarNode = stageBarNode.Next;
            }

            return null;
        }

        public LinkedListNode<ProgressBarStage> GetNode(Enum enumValue)
        {
            return GetNode(enumValue.ToString());
        }

        #endregion

        #region Add
        public ProgressBarStage Add(string keyName
            , string title
            , ProgressBarStageStatus status = ProgressBarStageStatus.Pending
            , ProgressBarStageAttributes attributes = ProgressBarStageAttributes.None)
        {
            ProgressBarStage stage = new ProgressBarStage(title) { Status = status, Attributes = attributes };
            Add(stage);
            return stage;
        }

        public ProgressBarStage Add(Enum enumValue
            , string titleOverride = null
            , ProgressBarStageStatus status = ProgressBarStageStatus.Pending
            , ProgressBarStageAttributes attributes = ProgressBarStageAttributes.None)
        {
            ProgressBarStage stage = new ProgressBarStage(enumValue) { Status = status, Attributes = attributes };
            if (!titleOverride.IsNullOrBlank())
            { stage.Title = titleOverride; }

            Add(stage);
            return stage;
        }

        public ProgressBarStage AddOptional(Enum enumValue, string titleOverride = null, ProgressBarStageStatus status = ProgressBarStageStatus.Pending)
        {
            return Add(enumValue, titleOverride, status, ProgressBarStageAttributes.Optional);
        }

        public ProgressBarStage AddHidden(Enum enumValue, string titleOverride = null, ProgressBarStageStatus status = ProgressBarStageStatus.Pending)
        {
            return Add(enumValue, titleOverride, status, ProgressBarStageAttributes.Hidden);
        }

        public void AddRange(IEnumerable<ProgressBarStage> items)
        {
            items.ForEach(item => Add(item));
        }

        #endregion

        #region Status
        public bool InProgress
        {
            get { return this.Any(match => match.Status.IsCurrent()); }
        }
        public bool IsCompleted
        {
            get { return IsSucess || IsFailure; }
        }
        public bool IsSucess
        {
            get { return this.All(match => match.Status.IsCompleted()); }
        }
        public bool IsFailure
        {
            get { return !this.Any(match => match.Status.IsPending()) && this.Any(match => match.Status.IsFailure()); }
        }

        public void CompleteAll()
        {
            this.ForEach(stage => stage.Status = ProgressBarStageStatus.Completed);
        }

        #endregion

        #region ICollection<T> Members
        private readonly LinkedList<ProgressBarStage> _stages = new LinkedList<ProgressBarStage>();

        /// <summary>
        /// Gets the number of elements contained in the Collection<T>.
        /// </summary>
        public int Count 
        {
            get { return _stages.Count; } 
        }

        /// <summary>
        /// Adds an item. 
        /// </summary>
        /// <param name="item">The object to add to the Collection.</param>
        public void Add(ProgressBarStage newItem)
        {
            _stages.AddLast(newItem);
        }

        /// <summary>
        /// Determines whether Collection contains specific value. 
        /// </summary>
        /// <param name="item">The object to locate in the Collection.</param>
        /// <returns>true if item is found in the Collection; otherwise, false. </returns>
        public bool Contains(ProgressBarStage item)
        {
            return _stages.Contains(item);
        }

        /// <summary>
        /// Copies the elements to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">Destination one-dimensional array.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(ProgressBarStage[] array, int arrayIndex)
        {
            _stages.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets a value indicating whether the ICollection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes specified item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Return true of item was removed; false otherwise.</returns>
        public bool Remove(ProgressBarStage item)
        {
            if (_stages.Count > 0)
            {
                _stages.Remove(item);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes all objects
        /// </summary>
        public void Clear()
        {
            if (_stages.Count > 0)
            {
                _stages.Clear();
            }
        }

        public int FindIndex(Predicate<ProgressBarStage> predicate)
        {
            int index = 0;
            foreach (ProgressBarStage stage in this)
            {
                if (predicate(stage))
                {
                    return index;
                }
                index++;
            }
            
            return -1;
        }

        #endregion

        #region IEnumerable<T> Members
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<ProgressBarStage> GetEnumerator()
        {
            foreach (ProgressBarStage iter in _stages)
            {
                yield return iter;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (ProgressBarStage iter in _stages)
            {
                yield return iter;
            }
        }
        #endregion
    }

    #endregion


    #region Extensions
    public static class ProgressBarStageExtensions
    {
        public static void MarkCompleted(this IEnumerable<LinkedListNode<ProgressBarStage>> stageNodes)
        {
            stageNodes.Select(stageNode => stageNode.Value).MarkCompleted();
        }
        public static void MarkCompleted(this IEnumerable<ProgressBarStage> stages)
        {
            stages.ForEach(stage => stage.Status = ProgressBarStageStatus.Completed);
        }

        public static void SetAttributes(this IEnumerable<LinkedListNode<ProgressBarStage>> stageNodes, ProgressBarStageAttributes attributes)
        {
            stageNodes.Select(stageNode => stageNode.Value).SetAttributes(attributes);
        }
        public static void SetAttributes(this IEnumerable<ProgressBarStage> stages, ProgressBarStageAttributes attributes)
        {
            stages.ForEach(stage => stage.Attributes |= attributes);
        }

        public static bool ValidateNext(this LinkedListNode<ProgressBarStage> stageNode, string keyName)
        {
            var upcomingStages = stageNode.GetUpcoming();
            foreach (LinkedListNode<ProgressBarStage> upcomingStageNode in upcomingStages)
            {
                ProgressBarStage stage = upcomingStageNode.Value;
                if (stage.KeyName.Equals(keyName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (!stage.Attributes.IsOptional())
                {
                    return false;
                }
            }

            return false;
        }
        public static bool ValidateNext(this LinkedListNode<ProgressBarStage> stageNode, Enum enumValue)
        {
            return stageNode.ValidateNext(enumValue.ToString());
        }    
    }


    #endregion


    [DebuggerDisplay("{Title}")]
    public sealed class ProgressBarStage : IDynamicMarshaling
    {
        #region Constructors
        private ProgressBarStage()
        {
        }
        public ProgressBarStage(string keyName, string title = null) : this()
        {
            if (keyName.IsNullOrBlank()) throw new ArgumentNullException("title");
            KeyName = keyName;
            Title = !title.IsNullOrBlank() ? title : keyName;            
            Status = ProgressBarStageStatus.Pending;
        }
        public ProgressBarStage(Enum enumValue) : this(enumValue.ToString(), enumValue.DisplayName())
        {
            EnumValue = enumValue;
        }

        public static ProgressBarStage Completed(string name)
        {
            return new ProgressBarStage(name) { Status = ProgressBarStageStatus.Completed };
        }
        public static ProgressBarStage Current(string name)
        {
            return new ProgressBarStage(name) { Status = ProgressBarStageStatus.Current };
        }
        public static ProgressBarStage Pending(string name)
        {
            return new ProgressBarStage(name) { Status = ProgressBarStageStatus.Pending };
        }

        #endregion

        #region Properties
        public string Title { get; set; }
        public string KeyName { get; set; }
        public ProgressBarStageStatus Status { get; set; }
        public ProgressBarStageAttributes Attributes { get; set; }
        public Enum EnumValue { get; set; }

        private static readonly string EllipsisText = new String((char)0x2026, 1);
        public static readonly ProgressBarStage EllipsisCompleted = new ProgressBarStage(EllipsisText) { Status = ProgressBarStageStatus.Completed };
        public static readonly ProgressBarStage EllipsisPending = new ProgressBarStage(EllipsisText);

        #endregion

        #region Serialization
        dynamic IDynamicMarshaling.Write()
        {
            dynamic serialized = new System.Dynamic.ExpandoObject();
            serialized.Title = Title;
            serialized.KeyName = KeyName;
            serialized.Status = new { Value = Status.ToString(), Text = Status.DisplayName() };
            serialized.Attributes = new { Value = Attributes.ToString(), Attributes = Status.DisplayName() };
            return serialized;
        }

        #endregion

    }    
}
