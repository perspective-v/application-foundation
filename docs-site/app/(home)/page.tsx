import type { Metadata } from 'next';
import { notFound } from 'next/navigation';
import { getMDXComponents } from '@/components/mdx';
import { getSource } from '@/lib/source';

export const metadata: Metadata = {
  title: 'Application Foundation',
  description: 'Shared .NET libraries for PerspectiveV applications.',
};

export default async function HomePage() {
  const page = (await getSource()).getPage([]);
  if (!page) notFound();

  const { body } = await (await page.data.load()).render(getMDXComponents());

  return (
    <main className="container mx-auto flex flex-col gap-8 px-4 py-12">
      {body}
    </main>
  );
}
