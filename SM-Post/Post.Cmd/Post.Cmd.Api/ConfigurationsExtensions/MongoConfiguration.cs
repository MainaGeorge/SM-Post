using CQRS.Core.Events;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Post.Cmd.Api.ConfigurationsExtensions;

public static class MongoConfiguration
{
    public static void RegisterEventClassMaps(params Type[] markerTypes)
    {
        //Register Serializers
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        var baseType = typeof(BaseEvent);

        // Gather assemblies for BaseEvent AND all provided marker types
        var assembliesToScan = markerTypes
            .Select(t => t.Assembly)
            .Append(baseType.Assembly)
            .Distinct();

        // Discover concrete events across all target assemblies
        var derivedTypes = assembliesToScan
            .SelectMany(a => a.GetTypes())
            .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract && t != baseType)
            .ToList();

        // Configure BaseEvent as root class and add known concrete types
        if (!BsonClassMap.IsClassMapRegistered(baseType))
        {
            var baseClassMap = new BsonClassMap(baseType);
            baseClassMap.AutoMap();
            baseClassMap.SetIsRootClass(true);

            foreach (var derivedType in derivedTypes)
                baseClassMap.AddKnownType(derivedType);

            BsonClassMap.RegisterClassMap(baseClassMap);
        }

        // Register class map for each concrete event type
        foreach (var derivedType in derivedTypes)
        {
            if (!BsonClassMap.IsClassMapRegistered(derivedType))
            {
                var derivedClassMap = new BsonClassMap(derivedType);
                derivedClassMap.AutoMap();
                BsonClassMap.RegisterClassMap(derivedClassMap);
            }
        }
    }
}
