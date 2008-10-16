<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:template match="/">
  <html lang="en">
    <head>
      <title>Strongwind Test Status Dashboard</title>
      <link rel="stylesheet" type="text/css" href="dashboard.css" media="all"/>
    </head>
    <body>
      <p class="header">
        <span id="documentName">Strongwind Test Status Dashboard</span><br/>
      </p>
        <table>
        <tr>
          <th id="controlNumber"></th>
          <th id="controlName">Control</th>
          <th id="status">Status</th>
          <th id="time">Elapsed Time</th>
        </tr>
        <xsl:for-each select="dashboard/control">
          <tr>
            <td><xsl:number/></td>
            <td><xsl:value-of select="name"/></td>
            <xsl:variable name="status" select="status"/>
            <xsl:choose>
              <xsl:when test="$status = -1">
                <td>NA</td>
              </xsl:when>
              <xsl:when test="$status = 0">
                <td>Pass</td>
              </xsl:when>
              <xsl:otherwise>
                <td>Fail</td>
              </xsl:otherwise>
            </xsl:choose>
            <xsl:variable name="time" select="time"/>
            <xsl:choose>
              <xsl:when test="$status = 0">
                <td><xsl:value-of select="time"/></td>
              </xsl:when>
              <xsl:otherwise>
                <td>NA</td>
              </xsl:otherwise>
            </xsl:choose>
          </tr>
        </xsl:for-each>
      </table>
    </body>
  </html>
</xsl:template>
</xsl:stylesheet>

