<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:template match="/">
    <html lang="en">
                <xsl:variable name="control" select="regression/controlName"/>
                <xsl:variable name="controlLower" select="translate($control, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')"/>
        <head>
            <title><xsl:value-of select="$control"/> Regression Test</title>
            <link rel="stylesheet" type="text/css" href="../control.css" media="all"/>
        </head>
        <body>
            <div class="header">
                <p>
                    <span id="documentName">
                        <xsl:value-of select="$control"/> Regression Test
                    </span>
                </p>
            </div>
            <div class="subheader">
                <p>
                    <span id="lastUpdated">
                        Last Updated: <xsl:value-of select="regression/timeAndDate"/>
                    </span>
                </p>
            </div>
            <div class="regression">
                <xsl:comment>Convert the number of seconds it took the regression test
                    to run into days, hours, minutes, and seconds</xsl:comment>
                <xsl:variable name="numDays" select="floor(number(regression/elapsedTime) div 86400)"/>
                <xsl:variable name="tmpSec1" select="number(regression/elapsedTime) mod 86400"/>
                <xsl:variable name="numHours" select="floor($tmpSec1 div 3600)"/>
                <xsl:variable name="tmpSec2" select="$tmpSec1 mod 3600"/>
                <xsl:variable name="numMinutes" select="floor($tmpSec2 div 60)"/>
                <xsl:variable name="numSeconds" select="format-number(number(regression/elapsedTime) mod 60, '#.#')"/>
                <p>
                    <span id="avgTime">Approximate Test Time: </span>
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
                        <th id="number"></th>
                        <th id="machine">Machine</th>
                        <th id="status">Status</th>
                        <th id="time">Elapsed Time</th>
                    </tr>
                    <xsl:for-each select="regression/machine">
                        <tr>
                            <td><xsl:number/></td>
                            <xsl:variable name="name" select="name"/>
                            <td>
                                <a href="../../logs/regression/{$controlLower}/{$name}">
                                <xsl:value-of select="$name"/>
                                </a>
                            </td>
                            <xsl:variable name="status" select="status"/>
                            <xsl:choose>
                                <xsl:when test="$status = -1">
                                    <td><center><img src="../grey.png" alt="not run" title="not run" height="22" width="22"/></center></td>
                                </xsl:when>
                                <xsl:when test="$status = 0">
                                    <td><center><img src="../green.png" alt="pass" title="pass" height="22" width="22"/></center></td>
                                </xsl:when>
                                <xsl:otherwise>
                                    <td><center><img src="../red.png" alt="fail" title="fail" height="22" width="22"/></center></td>
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
