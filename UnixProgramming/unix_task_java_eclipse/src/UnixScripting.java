import java.util.*;

/**
 * Created with IntelliJ IDEA.
 * User: Deliany
 * Date: 27.03.13
 * Time: 03:08
 * To change this template use File | Settings | File Templates.
 */
public class UnixScripting {
    public static Map<String,Integer> ProcessContent(String content)
    {
        // 1. convert all letters to upper case and replace spaces to new lines
        String contentString = content.toString();
        contentString = contentString.replace(' ', '\n');
        contentString = contentString.toUpperCase();

        List<String> words = new ArrayList<String>();
        for(String word : contentString.split("\n"))
        {
            words.add(word);
        }

        // 2. sort the result
        Collections.sort(words);

        // 3. count unique words
        Map<String, Integer> wordCount = new HashMap<String, Integer>();
        for(String word : words)
        {
            if (!wordCount.containsKey(word)) {
                wordCount.put(word, new Integer(1));
            }
            else {
                wordCount.put(word, new Integer(wordCount.get(word)+1));
            }
        }

        // 4. sort the result reversive
        wordCount = sortByValue(wordCount);
        return wordCount;
    }

    public static Map<String, Integer> sortByValue(Map<String, Integer> map) {
        List<Map.Entry<String, Integer>> list = new LinkedList<Map.Entry<String, Integer>>(map.entrySet());

        Collections.sort(list, new Comparator<Map.Entry<String, Integer>>() {

            public int compare(Map.Entry<String, Integer> m1, Map.Entry<String, Integer> m2) {
                return (m2.getValue()).compareTo(m1.getValue());
            }
        });

        Map<String, Integer> result = new LinkedHashMap<String, Integer>();
        for (Map.Entry<String, Integer> entry : list) {
            result.put(entry.getKey(), entry.getValue());
        }
        return result;
    }
}
