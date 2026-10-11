import { localMd } from '@fumadocs/local-md';
import { watchWithDevServer } from '@fumadocs/local-md/dev/ws';
import { llms, dynamicLoader } from 'fumadocs-core/source';
import { docsRoute } from './shared';

const docs = localMd({
  dir: '..',
  include: ['index.mdx', 'docs/**/*.{md,mdx}'],
});

if (process.env.NODE_ENV === 'development') void watchWithDevServer(docs);

const docsLoader = dynamicLoader(docs.dynamicSource(), {
  baseUrl: docsRoute,
  slugs: (file, next) => file.path.startsWith('docs/') ? next().slice(1) : next(),
  url: (slugs) => slugs.length === 0 ? '/' : docsRoute + '/' + slugs.join('/'),
});

export function getSource() {
  return docsLoader.get();
}

export function getDocsLlms(source: Awaited<ReturnType<typeof getSource>>) {
  return llms(source, {
    renderPage: (page) => '# ' + page.data.title + ' (' + page.url + ')\n\n' + page.data.content,
  });
}
