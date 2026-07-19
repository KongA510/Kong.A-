const fs = require('fs');
const remaining = JSON.parse(fs.readFileSync('remaining_english4.json', 'utf-8'));
// Group by length for efficient processing
const groups = {
  short: remaining.filter(p => p.length < 100),
  medium: remaining.filter(p => p.length >= 100 && p.length < 300),
  long: remaining.filter(p => p.length >= 300)
};
console.log(`Short (<100): ${groups.short.length}`);
console.log(`Medium (100-300): ${groups.medium.length}`);
console.log(`Long (300+): ${groups.long.length}`);
console.log(`Total unique: ${remaining.length}`);
// Show first 30 short ones
console.log('\n--- Short paragraphs ---');
groups.short.slice(0, 40).forEach((p, i) => console.log(`${i}: "${p}"`));
