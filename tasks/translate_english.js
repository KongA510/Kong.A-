const fs = require('fs');
const https = require('https');
const http = require('http');

const paras = JSON.parse(fs.readFileSync('english_paras.json', 'utf-8'));
const BATCH_SIZE = 5;
const DELAY_MS = 1200;
const PROGRESS_FILE = 'translate_progress.json';

// Load progress if exists
let translated = {};
if (fs.existsSync(PROGRESS_FILE)) {
  translated = JSON.parse(fs.readFileSync(PROGRESS_FILE, 'utf-8'));
}

function translateText(text) {
  return new Promise((resolve, reject) => {
    const encoded = encodeURIComponent(text.slice(0, 450));
    const url = `https://api.mymemory.translated.net/get?q=${encoded}&langpair=en|zh-CN`;
    https.get(url, {timeout: 10000}, (res) => {
      let data = '';
      res.on('data', c => data += c);
      res.on('end', () => {
        try {
          const json = JSON.parse(data);
          if (json.responseStatus === 200 && json.responseData.translatedText) {
            resolve(json.responseData.translatedText);
          } else {
            resolve(null);
          }
        } catch(e) { resolve(null); }
      });
    }).on('error', () => resolve(null));
  });
}

function sleep(ms) { return new Promise(r => setTimeout(r, ms)); }

async function main() {
  let done = 0;
  let failed = 0;
  const total = paras.length;

  for (let i = 0; i < paras.length; i++) {
    const key = `${paras[i].entry}_${i}`;
    if (translated[key]) { done++; continue; }

    const result = await translateText(paras[i].text);
    if (result && result !== paras[i].text && /[一-鿿]/.test(result)) {
      translated[key] = result;
      done++;
    } else {
      translated[key] = paras[i].text; // keep original as fallback
      failed++;
    }

    if ((i + 1) % BATCH_SIZE === 0) {
      // Save progress periodically
      fs.writeFileSync(PROGRESS_FILE, JSON.stringify(translated));
      process.stdout.write(`\rProgress: ${done}/${total} (${failed} failed)`);
      await sleep(DELAY_MS);
    }
  }

  fs.writeFileSync(PROGRESS_FILE, JSON.stringify(translated));
  console.log(`\nDone: ${done}/${total}, Failed: ${failed}`);
}

main().catch(console.error);
