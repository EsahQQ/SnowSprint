namespace _Project.Scripts.Libs.Factories
{
    public interface IFactory<T>
    {
        T Create();
    }
}