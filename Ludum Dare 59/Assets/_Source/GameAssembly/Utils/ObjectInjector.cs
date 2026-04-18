using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Utils
{
    public static class ObjectInjector
    {
        private static IObjectResolver _container;
        
        public static void Initialize(IObjectResolver container) => _container = container;

        public static void InjectGameObject(GameObject obj) => _container.InjectGameObject(obj);
        public static void InjectObject(object obj) => _container.Inject(obj);
    }
}