const fs = require('fs');
const https = require('https');

const remaining = JSON.parse(fs.readFileSync('remaining_english4.json', 'utf-8'));

async function translate(text) {
  return new Promise((resolve, reject) => {
    const q = encodeURIComponent(text.substring(0, 500));
    const url = `https://api.mymemory.translated.net/get?q=${q}&langpair=en|zh-CN`;

    https.get(url, {timeout: 15000}, (res) => {
      let data = '';
      res.on('data', c => data += c);
      res.on('end', () => {
        try {
          const j = JSON.parse(data);
          if (j.responseStatus === 200 && j.responseData.translatedText) {
            resolve(j.responseData.translatedText);
          } else {
            resolve(null);
          }
        } catch(e) { resolve(null); }
      });
    }).on('error', () => resolve(null)).on('timeout', function() { this.destroy(); resolve(null); });
  });
}

async function main() {
  // Only translate paragraphs between 50-500 chars (the sweet spot)
  const toTranslate = remaining.filter(p => p.length >= 50 && p.length <= 500);
  console.log(`Translating ${toTranslate.length} paragraphs (50-500 chars)...`);

  const translations = {};
  let translated = 0;
  let failed = 0;

  // Process in batches of 5 with delay
  for (let i = 0; i < toTranslate.length; i += 5) {
    const batch = toTranslate.slice(i, i + 5);
    const results = await Promise.all(batch.map(p => translate(p)));

    for (let j = 0; j < batch.length; j++) {
      if (results[j]) {
        translations[batch[j]] = results[j];
        translated++;
      } else {
        failed++;
      }
    }

    process.stdout.write(`\r${translated + failed}/${toTranslate.length} (${translated} ok, ${failed} failed)`);

    if (i + 5 < toTranslate.length) {
      await new Promise(r => setTimeout(r, 1200));
    }
  }

  console.log(`\nDone. ${translated} translated, ${failed} failed.`);

  // Save translations
  fs.writeFileSync('translations_api.json', JSON.stringify(translations, null, 2));

  // Apply to SQL
  const sql = fs.readFileSync('INSERT_系统管理详解_full.sql', 'utf-8');

  let replaced = 0;
  const result = sql.replace(/<Run xml:lang="zh-cn">([^<]+)<\/Run>/g, (match, text) => {
    if (/[一-鿿]/.test(text)) return match;
    if (text.startsWith('分类:') || text.startsWith('来源:')) return match;
    if (text.length < 4) return match;

    const trimmed = text.trim();
    if (translations[trimmed] || translations[text]) {
      replaced++;
      return `<Run xml:lang="zh-cn">${translations[trimmed] || translations[text]}</Run>`;
    }
    return match;
  });

  fs.writeFileSync('INSERT_系统管理详解_full.sql', result);
  console.log(`Applied ${replaced} translations to SQL.`);

  const rem = [...result.matchAll(/<Run xml:lang="zh-cn">([^<]+)<\/Run>/g)]
    .filter(m => !/[一-鿿]/.test(m[1]) && m[1].length > 4 && !m[1].startsWith('分类') && !m[1].startsWith('来源'));
  console.log(`Remaining unique English: ${[...new Set(rem.map(m => m[1]))].length}`);
}

main();
