using System;
using System.Collections.Generic;
using System.Reflection;
using Serilog;
using System.Linq;

namespace TaskBot
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DependencyAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class StatocDependencyAttribute : Attribute { }

    public class DependecyResolver
    {
        private readonly ILogger log;
        private readonly Dictionary<Type, Func<object>> dependencies;
        private readonly Dictionary<Type, object> staticDependecies;
        
        public DependecyResolver(ILogger log)
        {
            this.log = log;
            dependencies = new Dictionary<Type, Func<object>>();
            staticDependecies = new Dictionary<Type, object>();
        }
        

        public void ResolveDependecy(object dependentObject, PropertyInfo prop)
        {
            var dependecyType = prop.PropertyType;
            try 
            {
                var dependecyValue = dependencies[dependecyType]();
                prop.SetValue(dependentObject, dependecyValue);
            }
            catch(KeyNotFoundException)
            {
                log.Error(
                    "Can't find dependecy initialized for propery of type {dependecyType} in {objectType}.", 
                    dependecyType, 
                    dependentObject.GetType().Name);
            }
        }

        public void ResolveStaticDependecy(object dependentObject, PropertyInfo prop)
        {
            var dependecyType = prop.PropertyType;
            try 
            {
                var dependecyValue = staticDependecies[dependecyType];
                prop.SetValue(dependentObject, dependecyValue);
            }
            catch(KeyNotFoundException)
            {
                log.Fatal(
                    "Can't find static dependecy value for propery of type {dependecyType} in {objectType}.", 
                    dependecyType, 
                    dependentObject.GetType().Name);
            }
        }

        public void Resolve(object dependentObject)
        {
            log.Debug("Resolving dependecies for {type}", dependentObject.GetType().Name);
            var objectType = dependentObject.GetType();
            var props = objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var prop in props)
            {
                if (prop.CanWrite && prop.GetCustomAttribute<DependencyAttribute>() != null)
                {
                    ResolveDependecy(dependentObject, prop);
                }

                if (prop.CanWrite && prop.GetCustomAttribute<StatocDependencyAttribute>() != null)
                {
                    ResolveStaticDependecy(dependentObject, prop);
                }
            }
        }

        public DependecyResolver Add<T>(Func<T> initFunc) where T : class
        {
            dependencies.Add(typeof(T), initFunc);
            return this;
        }

        public DependecyResolver Add<T>() where T : class, new()
        {
            dependencies.Add(typeof(T), () => new T());
            return this;
        }

        public DependecyResolver AddStatic<T>(T value) 
        {
            staticDependecies.Add(typeof(T), value);
            return this;
        }
    }

    internal static class DI
    {
        private static DependecyResolver resolver;
        
        public static void SetResolver(DependecyResolver resolver) 
        {
            DI.resolver = resolver;
        }

        public static T Resolve<T>(T o)
        {
            resolver.Resolve(o);
            return o;
        }
    }
}