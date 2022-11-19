namespace MMBot.Blazor.Server.Auth;

public static class SecurityHeadersDefinitions
{
    public static HeaderPolicyCollection GetHeaderPolicyCollection(bool isDev, string idpHost)
    {
        var policy = new HeaderPolicyCollection()
                .AddFrameOptionsDeny()
                .AddXssProtectionBlock()
                .AddContentTypeOptionsNoSniff()
                .AddReferrerPolicyStrictOriginWhenCrossOrigin()
                .AddCrossOriginOpenerPolicy(builder => builder.SameOrigin())
                .AddCrossOriginResourcePolicy(builder => builder.SameOrigin())
                .AddContentSecurityPolicy(builder =>
                {
                    _ = builder.AddObjectSrc().None();
                    _ = builder.AddBlockAllMixedContent();
                    _ = builder.AddImgSrc().Self().From("data:");
                    _ = builder.AddFormAction().Self().From(idpHost);
                    _ = builder.AddFontSrc().Self().OverHttps();
                    _ = builder.AddBaseUri().Self();
                    _ = builder.AddFrameAncestors().None();

                    if (!isDev)
                    {
                        _ = builder.AddStyleSrc().Self();

                        // due to Blazor
                        _ = builder.AddScriptSrc()
                               .Self()
                               .WithHash256("v8v3RKRPmN4odZ1CWM5gw80QKPCCWMcpNeOmimNL2AA=")
                               .UnsafeEval();
                    }

                    // disable script and style CSP protection if using Blazor hot reload
                    // if using hot reload, DO NOT deploy with an insecure CSP
                })
                .RemoveServerHeader()
                .AddPermissionsPolicy(builder =>
                {
                    _ = builder.AddAccelerometer().None();
                    _ = builder.AddAutoplay().None();
                    _ = builder.AddCamera().None();
                    _ = builder.AddEncryptedMedia().None();
                    _ = builder.AddFullscreen().All();
                    _ = builder.AddGeolocation().None();
                    _ = builder.AddGyroscope().None();
                    _ = builder.AddMagnetometer().None();
                    _ = builder.AddMicrophone().None();
                    _ = builder.AddMidi().None();
                    _ = builder.AddPayment().None();
                    _ = builder.AddPictureInPicture().None();
                    _ = builder.AddSyncXHR().None();
                    _ = builder.AddUsb().None();
                });

        if (!isDev)
        {
            _ = policy.AddCrossOriginEmbedderPolicy(builder => builder.RequireCorp());
            // maxage = one year in seconds
            _ = policy.AddStrictTransportSecurityMaxAgeIncludeSubDomains(maxAgeInSeconds: 60 * 60 * 24 * 365);
        }

        return policy;
    }
}
