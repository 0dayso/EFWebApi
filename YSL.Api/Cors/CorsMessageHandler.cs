using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http;
using System.Web.Http.Controllers;

public class CorsMessageHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //�õ�����Ŀ��Action��HttpActionDescriptor
        HttpMethod originalMethod = request.Method;
        bool isPreflightRequest = request.IsPreflightRequest();
        if (isPreflightRequest)
        {
            string method = request.Headers.GetValues("Access-Control-Request-Method").First();
            request.Method = new HttpMethod(method);
        }
        HttpConfiguration configuration = request.GetConfiguration();
        HttpControllerDescriptor controllerDescriptor = configuration.Services.GetHttpControllerSelector().SelectController(request);
        HttpControllerContext controllerContext = new HttpControllerContext(request.GetConfiguration(), request.GetRouteData(), request)
        {
            ControllerDescriptor = controllerDescriptor
        };
        HttpActionDescriptor actionDescriptor = configuration.Services.GetActionSelector().SelectAction(controllerContext);

        //����HttpActionDescriptor�õ�Ӧ�õ�CorsAttribute����
        CorsAttribute corsAttribute = actionDescriptor.GetCustomAttributes<CorsAttribute>().FirstOrDefault() ??
            controllerDescriptor.GetCustomAttributes<CorsAttribute>().FirstOrDefault();
        if (null != corsAttribute)
        {
            //����CorsAttributeʵʩ��Ȩ��������Ӧ��ͷ
            IDictionary<string, string> headers;
            request.Method = originalMethod;
            bool authorized = corsAttribute.TryEvaluate(request, out headers);
            HttpResponseMessage response;
            if (isPreflightRequest)
            {
                if (authorized)
                {
                    response = new HttpResponseMessage(HttpStatusCode.OK);
                }
                else
                {
                    response = request.CreateErrorResponse(HttpStatusCode.BadRequest, corsAttribute.ErrorMessage);
                }
            }
            else
            {
                response = base.SendAsync(request, cancellationToken).Result;
            }

            //�����Ӧ��ͷ
            foreach (var item in headers)
            {
                response.Headers.Add(item.Key, item.Value);
            }
            return Task.FromResult<HttpResponseMessage>(response);
        }
        else
        {
            return base.SendAsync(request, cancellationToken);
        }
    }
}


//GlobalConfiguration.Configuration.MessageHandlers.Add(new MyCorsMessageHandler());
public class MyCorsMessageHandler : DelegatingHandler
{
    protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //���ݵ�ǰ���󴴽�CorsRequestContext
        CorsRequestContext context = request.CreateCorsRequestContext();

        //��Է�Ԥ�����󣺽����󴫵ݸ���Ϣ����ܵ��������ּ����������õ���Ӧ
        HttpResponseMessage response = null;
        if (!context.IsPreflight)
        {
            response = await base.SendAsync(request, cancellationToken);
        }

        //����ע���CorsPolicyProviderFactory�õ���Ӧ��CorsPolicyProvider
        //������CorsPolicyProvider�õ���ʾCORS��Դ��Ȩ���Ե�CorsPolicy
        HttpConfiguration configuration = request.GetConfiguration();
        CorsPolicy policy = await configuration.GetCorsPolicyProviderFactory().GetCorsPolicyProvider(request).GetCorsPolicyAsync(request, cancellationToken);

        //��ȡע���CorsEngine
        //����CorsEngine������ʵʩCORS��Դ��Ȩ���飬���õ���ʾ��������CorsResult����
        ICorsEngine engine = configuration.GetCorsEngine();
        CorsResult result = engine.EvaluatePolicy(context, policy);

        //���Ԥ������
        //�������ͨ����Ȩ���飬����һ��״̬Ϊ��200�� OK������Ӧ�����CORS��ͷ
        //�����Ȩ����ʧ�ܣ�����һ��״̬Ϊ��400�� Bad Request������Ӧ��ָ����Ȩʧ��ԭ��
        if (context.IsPreflight)
        {
            if (result.IsValid)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.AddCorsHeaders(result);
            }
            else
            {
                response = request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Join(" |", result.ErrorMessages.ToArray()));
            }
        }
        //��Է�Ԥ������
        //CORS��ͷֻ����ͨ����Ȩ��������²Żᱻ��ӵ���Ӧ��ͷ������
        else if (result.IsValid)
        {
            response.AddCorsHeaders(result);
        }
        return response;
    }
}

