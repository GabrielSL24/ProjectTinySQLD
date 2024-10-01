using ApiInterface.Exceptions;
using ApiInterface.InternalModels;

namespace ApiInterface.Processors
{
    internal class ProcessorFactory
    {
        internal static IProcessor Create(Request request)
        {
            if (request.RequestType is RequestType.SQLSentence)
            {
                return new SQLSentenceProcessor(request);
            }
            throw new UnknowRequestTypeException();
        }
    }
}
