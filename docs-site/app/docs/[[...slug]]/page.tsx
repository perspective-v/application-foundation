import { getSource } from '@/lib/source';
import {
  DocsBody,
  DocsDescription,
  DocsPage,
  DocsTitle,
  MarkdownCopyButton,
  ViewOptionsPopover,
} from 'fumadocs-ui/layouts/docs/page';
import { notFound } from 'next/navigation';
import { getMDXComponents } from '@/components/mdx';
import type { Metadata } from 'next';
import { createRelativeLink } from 'fumadocs-ui/mdx';
import { getPageImageUrl, getPageMarkdownUrl, gitConfig } from '@/lib/shared';

type PageProps = { params: Promise<{ slug?: string[] }> };

export default async function Page(props: PageProps) {
  const params = await props.params;
  const source = await getSource();
  const page = source.getPage(params.slug);
  if (!page) notFound();

  const { body, toc } = await (await page.data.load()).render(
    getMDXComponents({
      a: createRelativeLink(source, page),
    }),
  );
  const markdownUrl = getPageMarkdownUrl(page).url;

  return (
    <DocsPage toc={toc} full={page.data.frontmatter.full}>
      <DocsTitle>{page.data.title}</DocsTitle>
      <DocsDescription className="mb-0">{page.data.description}</DocsDescription>
      <div className="flex flex-row gap-2 items-center border-b pb-6">
        <MarkdownCopyButton markdownUrl={markdownUrl} />
        <ViewOptionsPopover
          markdownUrl={markdownUrl}
          githubUrl={'https://github.com/' + gitConfig.user + '/' + gitConfig.repo + '/blob/' + gitConfig.branch + '/' + page.path}
        />
      </div>
      <DocsBody>
        {body}
      </DocsBody>
    </DocsPage>
  );
}

export async function generateStaticParams() {
  const source = await getSource();

  return source.getPages().filter((page) => page.slugs.length > 0).map((page) => ({
    slug: page.slugs,
  }));
}

export async function generateMetadata(props: PageProps): Promise<Metadata> {
  const params = await props.params;
  const source = await getSource();
  const page = source.getPage(params.slug);
  if (!page) notFound();

  return {
    title: page.data.title,
    description: page.data.description,
    openGraph: {
      images: getPageImageUrl(page).url,
    },
  };
}
