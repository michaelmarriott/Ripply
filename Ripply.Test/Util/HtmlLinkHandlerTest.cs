using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Ripply.Util;

namespace Ripply.Test.Util
{
    public class HtmlLinkHandlerTest
    {

        [Fact]
        public void GivenValidLink_WhenIsValidLinkExecuted_ShouldReturnTrue()
        {
            //Arrange
            var scrapper = new MockHtmlScrapper();
            var htmlLinkHandler = new HtmlLinkHandler(scrapper) { };
            //Act
            var validlink = "/store1/thisisatest";
            var result = htmlLinkHandler.IsValidLink(validlink);
            //Assert
            Assert.True(result);
        }

        [Fact]
        public void GivenInValidLink_WhenIsValidLinkExecuted_ShouldReturnFalse()
        {
            //Arrange
            var scrapper = new MockHtmlScrapper();
            var htmlLinkHandler = new HtmlLinkHandler(scrapper) { };
            //Act
            var validlink = "/unknown/thisisatest";
            var result = htmlLinkHandler.IsValidLink(validlink);
            //Assert
            Assert.False(result);
        }

    }
}
