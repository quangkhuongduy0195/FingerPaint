using System;
using System.Threading.Tasks;
using FingerPaint.Reponse;
using FingerPaint.Request;
using Refit;

namespace FingerPaint
{
    public interface IApiSaveFile
    {
        [Post("/api/saveFile")]
        Task<ReponseSave> SaveFile([Body] RequestSave request);

        [Post("/api/getFile")]
        Task<ReponseGet> GetFile([Body] RequestGet request);
    }
}
