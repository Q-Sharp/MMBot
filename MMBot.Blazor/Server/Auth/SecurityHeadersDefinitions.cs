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
                    builder.AddObjectSrc().None();
                    builder.AddBlockAllMixedContent();
                    builder.AddImgSrc()
                           .Self()
                           .From("data:")
                           .From("https://www.mmbot.xyz")
                           .From("https://cdn.discordapp.com");
                    builder.AddFormAction().Self().From(idpHost);
                    builder.AddFontSrc()
                           .Self()
                           .From("https://fonts.googleapis.com")
                           .From("https://www.mmbot.xyz")
                           .OverHttps();
                    builder.AddBaseUri()
                           .From("https://www.mmbot.xyz")
                           .Self();
                    builder.AddFrameAncestors().None();

                    if (!isDev)
                    {
                        builder.AddStyleSrc()
                               .Self()
                               .From("https://static.cloudflareinsights.com")
                               .From("https://www.mmbot.xyz")
                               .UnsafeInline();

                        // due to Blazor
                        builder.AddScriptSrc()
                               .Self()
                               .WithHash256("v8v3RKRPmN4odZ1CWM5gw80QKPCCWMcpNeOmimNL2AA=")
                               .From("https://ajax.googleapis.com/")
                               .From("https://static.cloudflareinsights.com/")
                               .From("https://www.mmbot.xyz/")
                               .UnsafeInline()
                               .UnsafeEval();
                    }

                    // disable script and style CSP protection if using Blazor hot reload
                    // if using hot reload, DO NOT deploy with an insecure CSP
                })
                .RemoveServerHeader()
                .AddPermissionsPolicy(builder =>
                {
                    builder.AddAccelerometer().None();
                    builder.AddAutoplay().None();
                    builder.AddCamera().None();
                    builder.AddEncryptedMedia().None();
                    builder.AddFullscreen().All();
                    builder.AddGeolocation().None();
                    builder.AddGyroscope().None();
                    builder.AddMagnetometer().None();
                    builder.AddMicrophone().None();
                    builder.AddMidi().None();
                    builder.AddPayment().None();
                    builder.AddPictureInPicture().None();
                    builder.AddSyncXHR().None();
                    builder.AddUsb().None();
                });

        if (!isDev)
        {
             policy.AddCrossOriginEmbedderPolicy(builder => builder.RequireCorp());
            // maxage = one year in seconds
             policy.AddStrictTransportSecurityMaxAgeIncludeSubDomains(maxAgeInSeconds: 60 * 60 * 24 * 365);
        }

        policy.ApplyDocumentHeadersToAllResponses();

        return policy;
    }
}
