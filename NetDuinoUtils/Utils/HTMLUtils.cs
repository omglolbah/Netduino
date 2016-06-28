using System;

namespace NetDuinoUtils.Utils
{
    public static class HTMLUtils
    {
        public static String BuildHTML(string content)
        {
            string returnString = @"
<html>
  <head>
    <style type=""text/css"">
    body{
        font-family: 'Courier New', Courier, monospace;
        background-color: #000000;
        color: #bbbbbb
    }
  </style>
  </head>
  <body>
    <pre>
    " + content +
  @"    </pre
  </body>
</html>";
            return returnString;
        }
    }
}
