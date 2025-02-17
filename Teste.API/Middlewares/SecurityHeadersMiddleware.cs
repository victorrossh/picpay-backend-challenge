namespace Teste.API.Middlewares;

public class SecurityHeadersMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Content Security Policy (CSP) - Enhanced with modern recommendations
        var csp = new List<string>
        {
            "default-src 'self'", // Default to same-origin
            "script-src 'self' 'sha256-xyz'", // Allow specific scripts (replace with actual hashes)
            "style-src 'self'", // Styles from same origin
            "img-src 'self' data:", // Images and data URIs
            "font-src 'self'", // Fonts from same origin
            "connect-src 'self'", // Restrict connections
            "frame-src 'none'", // Replace X-Frame-Options for modern browsers
            "object-src 'none'", // Block plugins
            "form-action 'self'", // Restrict form submissions
            "base-uri 'self'", // Prevent base tag hijacking
            "upgrade-insecure-requests" // Force HTTPS for HTTP requests
            // "report-uri /csp-report-endpoint"    // Enable reporting
        };

        context.Response.Headers["Content-Security-Policy"] = string.Join("; ", csp);

        // HSTS - Add comment about preload list commitment
        context.Response.Headers["Strict-Transport-Security"] =
            "max-age=31536000; includeSubDomains"; // Removed preload - only include if committed

        // Modern browser protections
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";

        // Clickjacking protection (redundant with CSP frame-src but included for legacy browsers)
        context.Response.Headers["X-Frame-Options"] = "DENY";

        // XSS Protection (legacy browsers only)
        context.Response.Headers["X-XSS-Protection"] = "0"; // Disable deprecated filter

        // Referrer Policy
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        // Permissions Policy (modern syntax)
        context.Response.Headers["Permissions-Policy"] = string.Join(", ", "accelerometer=()", "camera=()",
            "geolocation=()", "gyroscope=()", "magnetometer=()", "microphone=()", "payment=()", "usb=()");

        // Cache-Control - Apply carefully based on content type
        if (context.Response.ContentType?.StartsWith("text/html") == true)
            context.Response.Headers["Cache-Control"] = "no-store, max-age=0";

        await next(context);
    }
}