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
      <p class="updateStatus">
	<h3>Package Update Status</h3>
        <table>
          <tr>
            <th id="machine">Machine</th>
            <th id="update">Status</th>
          </tr>
          <xsl:for-each select="dashboard/updateStatus/machine">
            <tr>
              <xsl:variable name="machineName" select="name"/>
              <td><a href="../../logs/{$machineName}_package_status"><xsl:value-of select="$machineName"/></a></td>
              <xsl:variable name="status" select="status"/>
              <xsl:choose>
                <xsl:when test="$status = 0">
                  <td><center><img src="green.png" alt="fail" title="fail" height="22" width="22"/></center></td>
                </xsl:when>
                <xsl:otherwise>
                  <td><center><img src="red.png" alt="fail" title="fail" height="22" width="22"/></center></td>
                </xsl:otherwise>
              </xsl:choose>
            </tr>
          </xsl:for-each>
        </table>
      </p>
      <div class="smoke">
        <xsl:comment>Convert the number of seconds it took the smoke test to
        run into days, hours, minutes, and seconds</xsl:comment>
        <xsl:variable name="numDays" select="floor(number(dashboard/smoke/elapsedTime) div 86400)"/>
        <xsl:variable name="tmpSec1" select="number(dashboard/smoke/elapsedTime) mod 86400"/>
        <xsl:variable name="numHours" select="floor($tmpSec1 div 3600)"/>
        <xsl:variable name="tmpSec2" select="$tmpSec1 mod 3600"/>
        <xsl:variable name="numMinutes" select="floor($tmpSec2 div 60)"/>
        <xsl:variable name="numSeconds" select="format-number(number(dashboard/smoke/elapsedTime) mod 60, '#.#')"/>
        <h3>Smoke Tests</h3>
        <p class="summaryTop">
          <span id="percentPassed">Passed: </span>
          <xsl:value-of select="dashboard/smoke/percentPassed"/>
          <br/>
          <span id="regressionTime">Smoke Test Time: </span>
          <xsl:if test="$numDays > 0">
              <span id="days"><xsl:value-of select="$numDays"/></span>
            <xsl:choose>
              <xsl:when test="$numDays = 1">
                <span id="timeDays"> Day</span>
              </xsl:when>
              <xsl:otherwise>
                <span id="timeDays"> Days</span>
              </xsl:otherwise>
            </xsl:choose>
            <xsl:if test="($numHours > 0) or ($numMinutes > 0) or ($numSeconds > 0)">
              <span>, </span>
            </xsl:if>
          </xsl:if>
          <xsl:if test="$numHours > 0">
              <span id="hours"><xsl:value-of select="$numHours"/></span>
            <xsl:choose>
              <xsl:when test="$numHours = 1">
                <span id="timeHours"> Hour</span>
              </xsl:when>
              <xsl:otherwise>
                <span id="timeHours"> Hours</span>
              </xsl:otherwise>
            </xsl:choose>
            <xsl:if test="($numMinutes > 0) or ($numSeconds > 0)">
              <span>, </span>
            </xsl:if>
          </xsl:if>
          <xsl:if test="$numMinutes > 0">
              <span id="minutes"><xsl:value-of select="$numMinutes"/></span>
            <xsl:choose>
              <xsl:when test="$numMinutes = 1">
                <span id="timeMinutes"> Minute</span>
              </xsl:when>
              <xsl:otherwise>
                <span id="timeMinutes"> Minutes</span>
              </xsl:otherwise>
            </xsl:choose>
            <xsl:if test="$numSeconds > 0">
              <span>, </span>
            </xsl:if>
          </xsl:if>
          <xsl:if test="$numSeconds > 0">
              <span id="seconds"><xsl:value-of select="$numSeconds"/></span>
            <xsl:choose>
              <xsl:when test="numSeconds = 1">
                <span id="timeSeconds"> Second</span>
              </xsl:when>
              <xsl:otherwise>
                <span id="timeSeconds"> Seconds</span>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:if>
        </p>
        <table>
        <tr>
          <th id="controlNumber"></th>
          <th id="controlName">Control</th>
          <th id="status">Status</th>
          <th id="time">Elapsed Time</th>
        </tr>
        <xsl:for-each select="dashboard/smoke/control">
          <tr>
            <td><xsl:number/></td>
            <xsl:variable name="controlName" select="name"/>
            <xsl:variable name="controlNameLower" select="translate(name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')"/>
            <td><a href="../../logs/smoke/{$controlNameLower}"><xsl:value-of select="$controlName"/></a></td>
            <xsl:variable name="status" select="status"/>
            <xsl:choose>
              <xsl:when test="$status = -1">
                <td><center><img src="grey.png" alt="not run" title="not run" height="22" width="22"/></center></td>
              </xsl:when>
              <xsl:when test="$status = 0">
                <td><center><img src="green.png" alt="pass" title="pass" height="22" width="22"/></center></td>
              </xsl:when>
              <xsl:otherwise>
                <td><center><img src="red.png" alt="fail" title="fail" height="22" width="22"/></center></td>
              </xsl:otherwise>
            </xsl:choose>
            <xsl:variable name="time" select="time"/>
            <xsl:choose>
              <xsl:when test="$status = 0">
                <td><xsl:value-of select="time"/>s</td>
              </xsl:when>
              <xsl:otherwise>
                <td></td>
              </xsl:otherwise>
            </xsl:choose>
          </tr>
        </xsl:for-each>
        </table>
      </div>
      <!-- End of Smoke Test Portion -->
      <!-- Regression Test Portion -->
      <div class="regression">
        <xsl:comment>Convert the number of seconds it took the regression test
        to run into days, hours, minutes, and seconds</xsl:comment>
        <xsl:variable name="numDays" select="floor(number(dashboard/regression/elapsedTime) div 86400)"/>
        <xsl:variable name="tmpSec1" select="number(dashboard/regression/elapsedTime) mod 86400"/>
        <xsl:variable name="numHours" select="floor($tmpSec1 div 3600)"/>
        <xsl:variable name="tmpSec2" select="$tmpSec1 mod 3600"/>
        <xsl:variable name="numMinutes" select="floor($tmpSec2 div 60)"/>
        <xsl:variable name="numSeconds" select="format-number(number(dashboard/regression/elapsedTime) mod 60, '#.#')"/>
        <h3>Regression Tests</h3>
        <p class="summaryTop">
          <span id="percentPassed">Passed: </span>
          <xsl:value-of select="dashboard/regression/percentPassed"/>
          <br/>
          <span id="regressionTime">Regression Test Time: </span>
          <xsl:if test="$numDays > 0">
              <span id="days"><xsl:value-of select="$numDays"/></span>
            <xsl:choose>
              <xsl:when test="$numDays = 1">
                <span id="timeDays"> Day</span>
              </xsl:when>
              <xsl:otherwise>
                <span id="timeDays"> Days</span>
              </xsl:otherwise>
            </xsl:choose>
            <xsl:if test="($numHours > 0) or ($numMinutes > 0) or ($numSeconds > 0)">
              <span>, </span>
            </xsl:if>
          </xsl:if>
          <xsl:if test="$numHours > 0">
              <span id="hours"><xsl:value-of select="$numHours"/></span>
            <xsl:choose>
              <xsl:when test="$numHours = 1">
                <span id="timeHours"> Hour</span>
              </xsl:when>
              <xsl:otherwise>
                <span id="timeHours"> Hours</span>
              </xsl:otherwise>
            </xsl:choose>
            <xsl:if test="($numMinutes > 0) or ($numSeconds > 0)">
              <span>, </span>
            </xsl:if>
          </xsl:if>
          <xsl:if test="$numMinutes > 0">
              <span id="minutes"><xsl:value-of select="$numMinutes"/></span>
            <xsl:choose>
              <xsl:when test="$numMinutes = 1">
                <span id="timeMinutes"> Minute</span>
              </xsl:when>
              <xsl:otherwise>
                <span id="timeMinutes"> Minutes</span>
              </xsl:otherwise>
            </xsl:choose>
            <xsl:if test="$numSeconds > 0">
              <span>, </span>
            </xsl:if>
          </xsl:if>
          <xsl:if test="$numSeconds > 0">
              <span id="seconds"><xsl:value-of select="$numSeconds"/></span>
            <xsl:choose>
              <xsl:when test="numSeconds = 1">
                <span id="timeSeconds"> Second</span>
              </xsl:when>
              <xsl:otherwise>
                <span id="timeSeconds"> Seconds</span>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:if>
        </p>
        <table>
        <tr>
          <th id="controlNumber"></th>
          <th id="controlName">Control</th>
          <th id="status">Status</th>
          <th id="time">Elapsed Time</th>
        </tr>
        <xsl:for-each select="dashboard/regression/control">
          <tr>
            <td><xsl:number/></td>
            <xsl:variable name="controlName" select="name"/>
            <xsl:variable name="controlNameLower" select="translate(name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')"/>
            <td><a href="../../logs/regression/{$controlNameLower}"><xsl:value-of select="$controlName"/></a></td>
            <xsl:variable name="status" select="status"/>
            <xsl:choose>
              <xsl:when test="$status = -1">
                <td><center><img src="grey.png" alt="not run" title="not run" height="22" width="22"/></center></td>
              </xsl:when>
              <xsl:when test="$status = 0">
                <td><center><img src="green.png" alt="pass" title="pass" height="22" width="22"/></center></td>
              </xsl:when>
              <xsl:otherwise>
                <td><center><img src="red.png" alt="fail" title="fail" height="22" width="22"/></center></td>
              </xsl:otherwise>
            </xsl:choose>
            <xsl:variable name="time" select="time"/>
            <xsl:choose>
              <xsl:when test="$status = 0">
                <td><xsl:value-of select="time"/>s</td>
              </xsl:when>
              <xsl:otherwise>
                <td></td>
              </xsl:otherwise>
            </xsl:choose>
          </tr>
        </xsl:for-each>
        </table>
      </div>
    </body>
  </html>
</xsl:template>
</xsl:stylesheet>
