<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="html" indent="yes"/>
	<xsl:template match="/">
		<html>
			<head>
				<title>Publications</title>
				<style>
					table { width: 100%; border-collapse: collapse; }
					th, td { border: 1px solid black; padding: 8px; text-align: left; }
					th { background-color: #f2c372; }
				</style>
			</head>
			<body>
				<h1>Publications</h1>
				<table>
					<tr>
						<th>Title</th>
						<th>Authors</th>
						<th>Year</th>
						<th>Faculty</th>
						<th>Department</th>
					</tr>
					<xsl:for-each select="Publications/Publication">
						<tr>
							<td>
								<xsl:value-of select="Title"/>
							</td>
							<td>
								<xsl:value-of select="Authors"/>
							</td>
							<td>
								<xsl:value-of select="PublishedYear"/>
							</td>
							<td>
								<xsl:value-of select="Faculty"/>
							</td>
							<td>
								<xsl:value-of select="Department"/>
							</td>
						</tr>
					</xsl:for-each>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>