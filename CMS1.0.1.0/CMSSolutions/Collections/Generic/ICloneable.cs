namespace CMSSolutions.Collections.Generic
{
    public interface ICloneable<T>
    {
        T ShallowCopy();

        T DeepCopy();
    }
}