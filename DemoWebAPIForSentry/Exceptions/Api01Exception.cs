namespace DemoWebAPIForSentry.Exceptions
{
    [Serializable]
    public class Api01Exception : Exception
    {
        public Api01Exception() { }

        public Api01Exception(string name)
            : base(string.Format("Api01 Exception"))
        {

        }
    }
}
