namespace Mews.Fiscalization.Germany.Model
{
    public class ResponseResult<TResult, TErrorResult>
        where TResult : class
        where TErrorResult : class
    {
        internal ResponseResult(TResult successResult = null, TErrorResult errorResult = null)
        {
            SuccessResult = successResult;
            ErrorResult = errorResult;
        }

        public TResult SuccessResult { get; }

        public TErrorResult ErrorResult { get; }

        public bool IsSuccess 
        {
            get
            {
                return SuccessResult != null;
            }
        }
    }
}
