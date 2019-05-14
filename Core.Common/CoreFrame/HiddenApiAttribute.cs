using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace Core.Common.CoreFrame
{
    /// <summary>
    /// 隐藏接口，不生成到swagger文档展示
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public partial class HiddenApiAttribute : Attribute
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public class HiddenApiFilter : IDocumentFilter
    {
        /// <summary>
        /// 重写Apply方法，移除隐藏接口的生成
        /// </summary>
        /// <param name="swaggerDoc"></param>
        /// <param name="context"></param>
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var ignoreApis = context.ApiDescriptions.Where(wh => wh.ActionAttributes().Any(any => any is HiddenApiAttribute));
            if (ignoreApis != null)
            {
                foreach (var ignoreApi in ignoreApis)
                {
                    swaggerDoc.Paths.Remove("/" + ignoreApi.RelativePath);
                }
            }
        }
    }
}
