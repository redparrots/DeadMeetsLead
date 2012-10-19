//#define LOG_CONTENT
//#define LOG_CONTENT_CONSOLE
//#define LOG_CONTENT_ACQUIRE
//#define DEBUG_CONTENT
//#define VERIFY_META_RESOURCES
#define THREAD_SAFE_CONTENT
//#define CONTENT_STATISTICS
#if CONTENT_STATISTICS
#error CONTENT_STATISTICS doesn't display correct values, see issue 1865
#endif
#define HARD_RESET_ON_LOST_DEVICE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace Graphics.Content
{

    /// <summary>
    /// Marks a class or field useable as a meta resource. A meta resource has to implement: ICloneable, Equals and GetHashCode
    /// </summary>
    public class MetaResourceSurrogateAttribute : Attribute { }

    public class ContentPool
    {
        static ContentPool()
        {
            VerifyAllMetaResourceTypes();
#if LOG_CONTENT
            Common.ProgramConfigurationInformation.AddWarning(new Common.ProgramConfigurationWarning
            {
                Text = "LOG_CONTENT", 
                Type = Common.ProgramConfigurationWarningType.Performance,
                Importance = Common.Importance.Trivial
            });
#endif
#if LOG_CONTENT_CONSOLE
            Common.ProgramConfigurationInformation.AddWarning(new Common.ProgramConfigurationWarning
            {
                Text = "LOG_CONTENT_CONSOLE", 
                Type = Common.ProgramConfigurationWarningType.Performance,
                Importance = Common.Importance.Trivial
            });
#endif
#if LOG_CONTENT_ACQUIRE
            Common.ProgramConfigurationInformation.AddWarning(new Common.ProgramConfigurationWarning
            {
                Text = "LOG_CONTENT_ACQUIRE", 
                Type = Common.ProgramConfigurationWarningType.Performance,
                Importance = Common.Importance.Critical
            });
#endif
#if DEBUG_CONTENT
            Common.ProgramConfigurationInformation.AddWarning(new Common.ProgramConfigurationWarning
            {
                Text = "DEBUG_CONTENT", 
                Type = Common.ProgramConfigurationWarningType.Performance | Common.ProgramConfigurationWarningType.Stability,
                Importance = Common.Importance.Critical
            });
#endif
#if VERIFY_META_RESOURCES
            Common.ProgramConfigurationInformation.AddWarning(new Common.ProgramConfigurationWarning
            {
                Text = "VERIFY_META_RESOURCES", 
                Type = Common.ProgramConfigurationWarningType.Performance | Common.ProgramConfigurationWarningType.Stability,
                Importance = Common.Importance.Critical
            });
#endif
#if THREAD_SAFE_CONTENT
            Common.ProgramConfigurationInformation.AddWarning(new Common.ProgramConfigurationWarning
            {
                Text = "THREAD_SAFE_CONTENT",
                Type = Common.ProgramConfigurationWarningType.Stability,
                Importance = Common.Importance.Trivial
            });
#endif
#if CONTENT_STATISTICS
            Common.ProgramConfigurationInformation.AddWarning(new Common.ProgramConfigurationWarning
            {
                Text = "CONTENT_STATISTICS",
                Type = Common.ProgramConfigurationWarningType.Performance,
                Importance = Common.Importance.Trivial
            });
#endif
        }

        public ContentPool(SlimDX.Direct3D9.Device device)
        {
            this.Device9 = device;
            LogInit();
            InitStatistics();
            LoadMappersFromAssembly(typeof(Content.ContentPool).Assembly);
        }
        public ContentPool(SlimDX.Direct3D10.Device device)
        {
            this.Device10 = device;
            LogInit();
            InitStatistics();
            LoadMappersFromAssembly(typeof(Content.ContentPool).Assembly);
        }

        /// <summary>
        /// Returns the requested resource, specified by the MetaResource. The resource
        /// has to be Released once it's no longer used.
        /// </summary>
        public T Acquire<T>(MetaResourceBase metaResource) where T : class
        {
            VerifyMetaResource(metaResource);
            if (resetting) return ResetAcquire<T>(metaResource);
            else return InnerAcquire<T>(metaResource);
        }
        T InnerAcquire<T>(MetaResourceBase metaResource) where T : class
        {
            if (metaResource == null) return default(T);

            var key = new Common.Tuple<MetaResourceBase, Type>(metaResource, typeof(T));

            // This is a small optimization, just see if the last acquired object was the same as this one,
            // in that case return it. Turns out often around half the acquires can be handled like this
            if (Object.Equals(lastAcquiredMetaResource, metaResource))
            {
                OnAcquire(metaResource);
                lastAcquiredResource.References++;
                lastAcquiredResource.LastAcquired = pruneK;
                return (T)lastAcquiredResource.Object;
            }


            Resource o = null;
#if THREAD_SAFE_CONTENT
            lock(lockObject)
            {
#endif
                if (contentByMetaResourceAndType.TryGetValue(key, out o))
                {
                    OnAcquire(metaResource);
                    o.References++;
                    o.LastAcquired = pruneK;
                    key.First = (MetaResourceBase)metaResource.Clone();
                    lastAcquiredMetaResource = key;
                    lastAcquiredResource = o;
                    return (T)o.Object;
                }

            key.First = (MetaResourceBase)metaResource.Clone();
#if DEBUG_CONTENT
            if (key.First.GetHashCode() != metaResource.GetHashCode())
                throw new Exception("Error in meta resource Clone or GetHashCode");
            if (!Object.Equals(key.First, metaResource))
            {
                string diffs = "";
                key.First.ReportEqualsDiffs(metaResource, (d) => diffs += "\n" + d);
                throw new Exception("Error in meta resource Clone or Equals\n\nDiffs:\n" + diffs);
            }
#endif

            var mapperKey = new Common.Tuple<Type, Type>(metaResource.GetType(), typeof(T));
            object ob = mappers[mapperKey].Do(MetaResourceDo.Construct, metaResource, this, null);
            OnConstruct(metaResource, ob);
            if (ob == null)
            {
                lastAcquiredMetaResource = new Common.Tuple<MetaResourceBase,Type>(null, null);
                lastAcquiredResource = null;
                return (T)ob;
            }
#if DEBUG_CONTENT
            if (key.First.GetHashCode() != metaResource.GetHashCode())
                throw new Exception("MetaResource was changed during construction (No longer the same hash code)");
            if (!Object.Equals(key.First, metaResource))
                throw new Exception("MetaResource was changed during construction (No longer Equals)");
#endif

#if THREAD_SAFE_CONTENT
            //lock (lockObject)
            //{
#endif
                OnAcquire(metaResource);
                 contentByMetaResourceAndType.Add(key, o = new Resource
                {
                    MetaResource = metaResource,
                    Object = ob,
                    References = 1,
                    LastAcquired = pruneK,
                    FirstAcquired = pruneK
                });
#if DEBUG_CONTENT
                Resource debugR;
                if (contentByResource.TryGetValue(o.Object, out debugR))
                    throw new Exception("Content by resource old: " + debugR.MetaResource + " new: " + o.MetaResource);
#endif
                contentByResource.Add(o.Object, o);

#if THREAD_SAFE_CONTENT
            }
#endif

            lastAcquiredMetaResource = key;
            lastAcquiredResource = o;
            return (T)o.Object;
        }
        Common.Tuple<MetaResourceBase, Type> lastAcquiredMetaResource;
        Resource lastAcquiredResource;

        public void Release(object resource)
        {
            if (preventRelease) return;
            if (resource == null) return;

            if (lastAcquiredResource != null && lastAcquiredResource.Object == resource)
            {
                OnRelease();
                lastAcquiredResource.References--;
                return;
            }

            Resource r;
#if THREAD_SAFE_CONTENT
            lock (lockObject)
#endif
                if (contentByResource.TryGetValue(resource, out r))
                {
                    OnRelease();
                    r.References--;
                }

            // This is how it should be done, but due to mantis#0000144 we use the hack above
            // contentByResource[resource].References--;
        }

        /// <summary>
        /// Temporary acquire of an object which does not need to be Released
        /// The object can only be guaranteed to be valid until the next Prune
        /// </summary>
        public T Peek<T>(MetaResourceBase metaResource) where T : class
        {
            if (metaResource == null) return default(T);
            OnPeek(metaResource);
            VerifyMetaResource(metaResource);

            // This is a small optimization, just see if the last acquired object was the same as this one,
            // in that case return it. Turns out often around half the acquires can be handled like this
            if (Object.Equals(lastAcquiredMetaResource, metaResource))
            {
                lastAcquiredResource.LastAcquired = pruneK;
                return (T)lastAcquiredResource.Object;
            }

            var key = new Common.Tuple<MetaResourceBase, Type>(metaResource, typeof(T));

            Resource o = null;
#if THREAD_SAFE_CONTENT
            lock (lockObject)
#endif
                if (contentByMetaResourceAndType.TryGetValue(key, out o))
                {
                    o.LastAcquired = pruneK;
                    
                    key.First = (MetaResourceBase)metaResource.Clone();

                    lastAcquiredMetaResource = key;
                    lastAcquiredResource = o;
                    return (T)o.Object;
                }

            T ob;
            if (resetting) ob = ResetAcquire<T>(metaResource);
            else ob = Acquire<T>(metaResource);
            Release(ob);
            return ob;
        }


        /// <summary>
        /// Remove all dead objects
        /// </summary>
        public void Prune(float dtime)
        {
            pruneAcc += dtime;
            if (pruneAcc >= 1)
            {
                pruneAcc = 0;
                pruneK++;

                List<KeyValuePair<Common.Tuple<MetaResourceBase, Type>, Resource>> toRemove =
                    new List<KeyValuePair<Common.Tuple<MetaResourceBase, Type>, Resource>>();

#if THREAD_SAFE_CONTENT
                lock (lockObject)
                {
#endif
                    foreach (var r in contentByMetaResourceAndType)
                        if (r.Value.References <= 0 && r.Value.LastAcquired + pruneDelta < pruneK)
                        {
                            pruneToDestroy.Enqueue(r);
                            toRemove.Add(r);
                        }

                    foreach (var r in toRemove)
                    {
                        contentByResource.Remove(r.Value.Object);
                        contentByMetaResourceAndType.Remove(r.Key);
                    }
#if THREAD_SAFE_CONTENT
                }
#endif
            }
            else if(pruneToDestroy.Count > 0)
            {
                //int nClears = Math.Max(1, pruneToDestroy.Count / 10);
                int nClears = pruneToDestroy.Count;
                for(int x=0; x < nClears; x++)
                {
                    var r = pruneToDestroy.Dequeue();
                    OnDestroy(r.Value.MetaResource, r.Value.Object);
                    var key = new Common.Tuple<Type, Type>(r.Value.MetaResource.GetType(), r.Value.Object.GetType());
                    mappers[key].Do(MetaResourceDo.Release, r.Value.MetaResource, this, r.Value.Object);
                }
            }
        }
        float pruneAcc = 0;
        Queue<KeyValuePair<Common.Tuple<MetaResourceBase, Type>, Resource>> pruneToDestroy =
            new Queue<KeyValuePair<Common.Tuple<MetaResourceBase, Type>, Resource>>();

        public void LostDevice()
        {
            Log("", " *** LostDevice ********************************************", ""); 
            RemoveToPrunes();
#if HARD_RESET_ON_LOST_DEVICE
            Release();
#else
            
            preventRelease = true;
#if THREAD_SAFE_CONTENT
            lock (lockObject)
#endif
                foreach (Resource r in contentByMetaResourceAndType.Values)
                {
                    OnLostDevice(r.MetaResource, r.Object);
                    var key = new Common.Tuple<Type, Type>(r.MetaResource.GetType(), r.Object.GetType());
                    mappers[key].Do(MetaResourceDo.LostDevice, r.MetaResource, this, r.Object);
                }
            preventRelease = false;
#endif
        }

        public void ResetDevice()
        {
            Log("", " *** ResetDevice *******************************************", "");
#if HARD_RESET_ON_LOST_DEVICE
#else
            contentByResource.Clear();
            resetting = true;
            resetResourcesById = new Dictionary<Common.Tuple<MetaResourceBase, Type>, Resource>();
            List<Resource> cbmrat;
#if THREAD_SAFE_CONTENT
            lock(lockObject)
#endif
                cbmrat = new List<Resource>(contentByMetaResourceAndType.Values);
            foreach(var r in cbmrat)
            {
                var key = new Common.Tuple<MetaResourceBase, Type>(r.MetaResource, r.Object.GetType());

                if (resetResourcesById.ContainsKey(key))
                {
                    if(log != null)
                    log.Write("Skipping: " + r.MetaResource);
                    continue;
                }

                OnResetDevice(r.MetaResource, r.Object);
                var mapperKey = new Common.Tuple<Type, Type>(r.MetaResource.GetType(), r.Object.GetType());
                r.Object = mappers[mapperKey].Do(MetaResourceDo.ResetDevice, r.MetaResource, this, r.Object);
#if THREAD_SAFE_CONTENT
                lock (lockObject)
#endif
                    contentByResource.Add(r.Object, r);
                resetResourcesById.Add(key, r);
            }
            resetting = false;
#endif
        }
        T ResetAcquire<T>(MetaResourceBase metaResource) where T : class
        {
            if (metaResource == null) return default(T);


            var key = new Common.Tuple<MetaResourceBase, Type>((MetaResourceBase)metaResource.Clone(), typeof(T));

            Resource o;
            // This object has already been reset
            if (resetResourcesById.TryGetValue(key, out o)) return (T)o.Object;

            // This resource doesn't exists yet, create it
            bool ok = false;
#if THREAD_SAFE_CONTENT
            lock (lockObject)
#endif
                ok = contentByMetaResourceAndType.TryGetValue(key, out o);
            if(!ok) return InnerAcquire<T>(metaResource);

            var mapperKey = new Common.Tuple<Type, Type>(o.MetaResource.GetType(), o.Object.GetType());

            o.Object = mappers[mapperKey].Do(MetaResourceDo.ResetDevice, o.MetaResource, this, o.Object);
            OnResetAcquire(o.MetaResource, o.Object);
#if THREAD_SAFE_CONTENT
            lock (lockObject)
#endif
                contentByResource.Add(o.Object, o);
            resetResourcesById.Add(key, o);

            return (T)o.Object;
        }
        Dictionary<Common.Tuple<MetaResourceBase, Type>, Resource> resetResourcesById;

        public void Release()
        {
            Log(""," *** Release ***********************************************", "");
            preventRelease = true;
#if THREAD_SAFE_CONTENT
            lock (lockObject)
#endif
            foreach (var r in contentByMetaResourceAndType)
            {
                OnDestroy(r.Value.MetaResource, r.Value);
                var key = new Common.Tuple<Type, Type>(r.Value.MetaResource.GetType(), r.Value.Object.GetType());
                mappers[key].Do(MetaResourceDo.Release, r.Value.MetaResource, this, r.Value.Object);
            }
            RemoveToPrunes();
#if THREAD_SAFE_CONTENT
            lock (lockObject)
            {
#endif
                contentByMetaResourceAndType.Clear();
                contentByResource.Clear();
#if THREAD_SAFE_CONTENT
            }
#endif
            preventRelease = false;
        }

        void RemoveToPrunes()
        {
            foreach (var r in pruneToDestroy)
            {
                OnDestroy(r.Value.MetaResource, r.Value);
                var key = new Common.Tuple<Type, Type>(r.Value.MetaResource.GetType(), r.Value.Object.GetType());
                mappers[key].Do(MetaResourceDo.Release, r.Value.MetaResource, this, r.Value.Object);
            }
            pruneToDestroy.Clear();
        }

        public String Dump()
        {
            StringBuilder s = new StringBuilder();
            int i = 1, totalNReferences = 0;
            s.AppendLine("#".PadLeft(4) + "| " + "Resource type".PadRight(25) + " References Age   MetaResource");
            foreach (var v in contentByMetaResourceAndType)
            {
                totalNReferences += v.Value.References;
                s.Append((i++).ToString().PadLeft(4)).Append("| ").
                    Append(v.Value.Object.GetType().Name.PadRight(25))
                    .Append(" ").Append(v.Value.References.ToString().PadRight(10))
                    .Append(" ").Append((pruneK - v.Value.FirstAcquired).ToString().PadRight(5))
                    .Append(" ").Append(v.Value.MetaResource).AppendLine();
            }
            s.AppendLine().Append("Total number of references: ").Append(totalNReferences).AppendLine();
            return s.ToString();
        }

        public void LoadMappersFromAssembly(Assembly assembly)
        {
            Type metaMapperBase = typeof(MetaMapperBase);
            Type metaMapper = typeof(MetaMapper<,>);
            foreach (Type t in assembly.GetTypes())
            {
                if (t.IsSubclassOf(metaMapperBase) && !t.ContainsGenericParameters)
                {
                    var type = t;
                    while (!type.IsGenericType || type.GetGenericTypeDefinition() != metaMapper)
                        type = type.BaseType;
                    var types = type.GetGenericArguments();
                    SetMapper((MetaMapperBase)Activator.CreateInstance(t), types[0], types[1]);
                }
            }
        }

        public void SetMapper<MetaT, T>(MetaMapper<MetaT, T> mapper)
        {
            SetMapper(mapper, typeof(MetaT), typeof(T));
        }

        public void SetMapper(MetaMapperBase mapper, Type metaType, Type resourceType)
        {
            var key = new Common.Tuple<Type, Type>(metaType, resourceType);
            MetaMapperBase oldMapper;
            if (mappers.TryGetValue(key, out oldMapper))
            {
                foreach (var v in contentByMetaResourceAndType.Values)
                    if (v.MetaResource.GetType() == metaType &&
                        v.Object.GetType() == resourceType)
                    {
                        oldMapper.Do(MetaResourceDo.Release, v.MetaResource, this, v.Object);
                        v.Object = mapper.Do(MetaResourceDo.Construct, v.MetaResource, this, null);
                    }
            }
            mappers[key] = mapper; 
        }

        public void RemoveMapper(Type metaType, Type resourceType)
        {
            var key = new Common.Tuple<Type, Type>(metaType, resourceType);
            mappers.Remove(key);

            List<Common.Tuple<MetaResourceBase, Type>> toRemove = new List<Common.Tuple<MetaResourceBase, Type>>();

            foreach (var v in contentByMetaResourceAndType)
                if (v.Value.MetaResource.GetType() == metaType &&
                    v.Value.Object.GetType() == resourceType)
                {
                    toRemove.Add(v.Key);
                    contentByResource.Remove(v.Value.Object);
                }

            foreach (var v in toRemove)
                contentByMetaResourceAndType.Remove(v);
        }

        public String ContentPath, ContentCachePath;
        public SlimDX.Direct3D9.Device Device9;
        public SlimDX.Direct3D10.Device Device10;

        #region Verification
        [System.Diagnostics.Conditional("VERIFY_META_RESOURCES")]
        static void VerifyMetaResource(MetaResourceBase metaResource)
        {
            VerifyMetaResourceType(metaResource);
            VerifyMetaResourceClone(metaResource);
        }
        
        /// <summary>
        /// A meta resource's properties must be either value types or implement ICloneable.
        /// This method enforces this.
        /// </summary>
        [System.Diagnostics.Conditional("VERIFY_META_RESOURCES")]
        static void VerifyMetaResourceType(MetaResourceBase metaResource)
        {
            if (metaResource == null) return;
            VerifyMetaResourceType(metaResource.GetType(), (error) => { throw new Exception(error); });
        }
        [System.Diagnostics.Conditional("VERIFY_META_RESOURCES")]
        static void VerifyMetaResourceType(Type type, Action<String> error)
        {
            foreach (var v in type.GetProperties())
            {
                if (v.PropertyType.IsValueType) continue;
                if (v.PropertyType == typeof(string)) continue;

                bool metaResourceSurrogate = false;
                foreach (var h in Attribute.GetCustomAttributes(v, true))
                    if (h is MetaResourceSurrogateAttribute) { metaResourceSurrogate = true; break; }

                if (metaResourceSurrogate) // The property has been marked as a meta resource surrogate, we don't check it since it's not a real meta resource
                    continue;

                if (typeof(MetaResourceBase).IsAssignableFrom(v.PropertyType))
                {
                    VerifyMetaResourceType(v.PropertyType, error);
                    continue;
                }

                if (v.PropertyType.IsGenericType && 
                    typeof(DataLink<>).IsAssignableFrom(v.PropertyType.GetGenericTypeDefinition()))
                    continue;

                error("Meta resource " + type.Name + " has a property " + v.Name + " which is neither a value type, meta resource or data link");
            }
        }
        /// <summary>
        /// Verifies that the Clone() method really performs a deep copy
        /// </summary>
        [System.Diagnostics.Conditional("VERIFY_META_RESOURCES")]
        static void VerifyMetaResourceClone(MetaResourceBase metaResource)
        {
            if (metaResource == null) return;
            VerifyMetaResourceClone(metaResource, (MetaResourceBase)metaResource.Clone());
        }
        static void VerifyMetaResourceClone(MetaResourceBase original, MetaResourceBase copy)
        {
            bool equals = Object.Equals(original, copy);
            foreach (var v in original.GetType().GetProperties())
            {
                if (typeof(MetaResourceBase).IsAssignableFrom(v.PropertyType))
                {
                    var o = (MetaResourceBase)((MetaResourceBase)v.GetValue(original, null));
                    if (o != null)
                    {
                        var c = (MetaResourceBase)o.Clone();
                        VerifyMetaResourceClone(o, c);
                    }
                }
            }
            if (!equals) throw new Exception(original + " does not equal " + copy);
        }

        [System.Diagnostics.Conditional("VERIFY_META_RESOURCES")]
        public static void VerifyAllMetaResourceTypes()
        {
            StringBuilder s = new StringBuilder();
            foreach(var v in typeof(ContentPool).Assembly.GetTypes())
                if (typeof(MetaResourceBase).IsAssignableFrom(v))
                {
                    VerifyMetaResourceType(v, (error) => { s.Append(v.Name).Append(" : ").Append(error).AppendLine(); });
                }
            if(System.IO.File.Exists("MetaResourceTypeErrors.txt"))
                System.IO.File.Delete("MetaResourceTypeErrors.txt");
            System.IO.File.AppendAllText("MetaResourceTypeErrors.txt", s.ToString());
        }
        #endregion

        #region Log & Statistics
        [System.Diagnostics.Conditional("LOG_CONTENT")]
        void LogInit()
        {
#if LOG_CONTENT_CONSOLE
            log = new MultiLogger(new TextLogger("ContentLog"), new ConsoleLogger());
#else
            log = new TextLogger("ContentLog");
#endif
            log.WriteRaw(
                    "Date: " + Application.ProgramStartTime.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n" +
                    "Op legend:\r\n + Construct\r\n - Release\r\n l OnLostDevice\r\n r OnResetDevice\r\n\r\n");
            log.WriteRaw("Time        | Op " +
                    "ObjectHashCode MetaResourceKey MetaResource\r\n");
        }


        [System.Diagnostics.Conditional("LOG_CONTENT")]
        void Log(params String[] msg)
        {
            log.Write(msg);
        }

        void OnAcquire(MetaResourceBase resource)
        {
#if LOG_CONTENT_ACQUIRE
            log.Write("a  " +
                //(ob != null ? ob.GetHashCode().ToString() : "null").PadRight(14) +
                       " " + resource);
#endif
#if CONTENT_STATISTICS
            pcTotalNumberOfReferences.Increment();
#endif
        }

        void OnRelease()
        {
#if LOG_CONTENT_ACQUIRE
            log.Write("r  " +
                //(ob != null ? ob.GetHashCode().ToString() : "null").PadRight(14) +
                       " ");
#endif
#if CONTENT_STATISTICS
            pcTotalNumberOfReferences.Decrement();
#endif
        }

        void OnPeek(MetaResourceBase resource)
        {
#if LOG_CONTENT_ACQUIRE
            log.Write("p  " +
                //(ob != null ? ob.GetHashCode().ToString() : "null").PadRight(14) +
                    " " + resource);      
#endif
        }

        void OnConstruct(MetaResourceBase resource, object ob)
        {
#if LOG_CONTENT
            log.Write("+  " +
                    (ob != null ? ob.GetHashCode().ToString() : "null").PadRight(14) + " " +
                    (ob != null ? ob.GetType().Name : "null").PadRight(14) +
                    " " + resource);
#endif
#if CONTENT_STATISTICS
            pcNumberOfObjects.Increment();
#endif
        }

        void OnDestroy(MetaResourceBase resource, object ob)
        {
#if LOG_CONTENT
            log.Write("-  " +
                    (ob != null ? ob.GetHashCode().ToString() : "null").PadRight(14) +
                    " " + resource);
#endif
#if CONTENT_STATISTICS
            pcNumberOfObjects.Decrement();
#endif
        }

        void OnLostDevice(MetaResourceBase resource, object ob)
        {
#if LOG_CONTENT
            log.Write("l  " +
                    (ob != null ? ob.GetHashCode().ToString() : "null").PadRight(14) +
                    " " + resource);
#endif
        }

        void OnResetDevice(MetaResourceBase resource, object ob)
        {
#if LOG_CONTENT
            log.Write("r  " +
                    (ob != null ? ob.GetHashCode().ToString() : "null").PadRight(14) +
                    " " + resource);
#endif
        }

        void OnResetAcquire(MetaResourceBase resource, object ob)
        {
#if LOG_CONTENT
            log.Write("r+ " +
                    (ob != null ? ob.GetHashCode().ToString() : "null").PadRight(14) +
                    " " + resource);
#endif
        }
        ILogger log;

        [System.Diagnostics.Conditional("CONTENT_STATISTICS")]
        void InitStatistics()
        {
            CounterCreationDataCollection counters = new CounterCreationDataCollection();

            counters.Add(new CounterCreationData
            {
                CounterName = "Number of objects",
                CounterHelp = "Number of objects in the content pool",
                CounterType = PerformanceCounterType.NumberOfItems32
            });

            counters.Add(new CounterCreationData
            {
                CounterName = "Total number of references",
                CounterHelp = "Total number of references in the content pool",
                CounterType = PerformanceCounterType.NumberOfItems32
            });

            if(PerformanceCounterCategory.Exists("KeldynContent"))
                PerformanceCounterCategory.Delete("KeldynContent");
            var c = PerformanceCounterCategory.Create("KeldynContent",
                "", PerformanceCounterCategoryType.SingleInstance, counters);

            pcNumberOfObjects = c.GetCounters()[0];
            pcNumberOfObjects.ReadOnly = false;
            pcTotalNumberOfReferences = c.GetCounters()[1];
            pcTotalNumberOfReferences.ReadOnly = false;
        }
        PerformanceCounter pcNumberOfObjects, pcTotalNumberOfReferences;
        #endregion

        class Resource
        {
            public MetaResourceBase MetaResource;
            public object Object;
            public int References, LastAcquired, FirstAcquired;
            public bool Destroyed = false;
        }

        Dictionary<Common.Tuple<MetaResourceBase, Type>, Resource> contentByMetaResourceAndType =
            new Dictionary<Common.Tuple<MetaResourceBase, Type>, Resource>();
        Dictionary<object, Resource> contentByResource = new Dictionary<object, Resource>();

        Dictionary<Common.Tuple<Type, Type>, MetaMapperBase> mappers = 
            new Dictionary<Common.Tuple<Type, Type>, MetaMapperBase>();
        
#if THREAD_SAFE_CONTENT
        object lockObject = new object();
#endif

        // pruneDelta determins how many prunes have to be run before a zero reference object will
        // actually be deleted, pruneK is a counter that increases each time we prune
        int pruneK = 0, pruneDelta = 20;

        bool preventRelease = false;
        bool resetting = false;
    }
}
