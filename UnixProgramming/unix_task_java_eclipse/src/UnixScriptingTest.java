import org.junit.Test;

import static junit.framework.Assert.assertEquals;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Map;

/**
 * Created with IntelliJ IDEA.
 * User: Deliany
 * Date: 27.03.13
 * Time: 03:30
 * To change this template use File | Settings | File Templates.
 */
public class UnixScriptingTest {
    @Test
    public void testProcessContent() throws Exception {
        String contentToParse = "The quick brown fox jumps over the lazy dog.";
        Map<String,Integer> wordCount = UnixScripting.ProcessContent(contentToParse);
        assertEquals("Parsed words count are not equal to expected.",wordCount.size(), 8);
        for (Map.Entry<String, Integer> entry : wordCount.entrySet())
        {
            if (entry.getKey().equals("THE")) {
                assertEquals(String.format("Parsed count of word '%s' are not equal to expected.", entry.getKey()),entry.getValue().intValue(), 2);
            }
            else {
                assertEquals(String.format("Parsed count of word '%s' are not equal to expected.", entry.getKey()),entry.getValue().intValue(), 1);
            }
        }
    }

    @Test
    public void testProcessContent2() throws Exception {
        String contentToParse = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat. Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat. Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi. Nam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer possim assum. Typi non habent claritatem insitam; est usus legentis in iis qui facit eorum claritatem. Investigationes demonstraverunt lectores legere me lius quod ii legunt saepius. Claritas est etiam processus dynamicus, qui sequitur mutationem consuetudium lectorum. Mirum est notare quam littera gothica, quam nunc putamus parum claram, anteposuerit litterarum formas humanitatis per seacula quarta decima et quinta decima. Eodem modo typi, qui nunc nobis videntur parum clari, fiant sollemnes in futurum.";
        Map<String,Integer> wordCount = UnixScripting.ProcessContent(contentToParse);
        assertEquals("Parsed words count are not equal to expected.",wordCount.size(), 149);
        assertEquals(String.format("Parsed count of word '%s' are not equal to expected.", "QUI"),wordCount.get("QUI").intValue(), 4);
        assertEquals(String.format("Parsed count of word '%s' are not equal to expected.", "QUI"),wordCount.get("IN").intValue(), 4);
        assertEquals(String.format("Parsed count of word '%s' are not equal to expected.", "QUI"),wordCount.get("UT").intValue(), 3);
        assertEquals(String.format("Parsed count of word '%s' are not equal to expected.", "QUI"),wordCount.get("ET").intValue(), 3);

        List<String> correctWordsOrder = new ArrayList<String>(Arrays.asList("QUI","IN","UT","ET","DOLORE","EST","QUOD","DOLOR","NOBIS","DUIS"));
        List keys = new ArrayList(wordCount.keySet());
        for (int i = 0; i < correctWordsOrder.size(); ++i)
        {
            String correctWord = correctWordsOrder.get(i);
            String givenWord = keys.get(i).toString();
            assertEquals(String.format("Expected word '%s' at %d position, but was was '%s'",correctWord,i,givenWord),
                    correctWord,givenWord);
        }
    }
}
