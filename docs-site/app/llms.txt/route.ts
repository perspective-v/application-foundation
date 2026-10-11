import { getDocsLlms, getSource } from '@/lib/source';

export const revalidate = false;

export async function GET() {
  const docsLlms = getDocsLlms(await getSource());
  return new Response(await docsLlms.index());
}
