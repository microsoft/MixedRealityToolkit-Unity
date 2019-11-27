#include "private/gc_priv.h"

static struct hblk* GetNextFreeBlock(ptr_t ptr)
{
	struct hblk* result = NULL;
	unsigned i;

	for (i = 0; i < N_HBLK_FLS + 1; i++)
	{
		struct hblk* freeBlock = GC_hblkfreelist[i];

		for (freeBlock = GC_hblkfreelist[i]; freeBlock != NULL; freeBlock = HDR(freeBlock)->hb_next)
		{
			/* We're only interested in pointers after "ptr" argument */
			if ((ptr_t)freeBlock < ptr)
				continue;

			/* If we haven't had a result before or our previous result is */
			/* ahead of the current freeBlock, mark the current freeBlock as result */
			if (result == NULL || result > freeBlock)
				result = freeBlock;
		}
	}

	return result;
}

void GC_foreach_heap_section(void* user_data, GC_heap_section_proc callback)
{
	unsigned i;

	GC_ASSERT(I_HOLD_LOCK());

	if (callback == NULL)
		return;

	for (i = 0; i < GC_n_heap_sects; i++)
	{
		ptr_t sectionStart = GC_heap_sects[i].hs_start;
		ptr_t sectionEnd = sectionStart + GC_heap_sects[i].hs_bytes;

		/* Merge in contiguous sections. Copied from GC_dump_regions

		A free block might start in one heap section and extend
		into the next one. Merging the section avoids crashes when
		trying to copy the start of section that is a free block
		continued from the previous section. */
		while (i + 1 < GC_n_heap_sects && GC_heap_sects[i + 1].hs_start == sectionEnd)
		{
			++i;
			sectionEnd = GC_heap_sects[i].hs_start + GC_heap_sects[i].hs_bytes;
		}

		while (sectionStart < sectionEnd)
		{
			struct hblk* nextFreeBlock = GetNextFreeBlock(sectionStart);

			if (nextFreeBlock == NULL || (ptr_t)nextFreeBlock > sectionEnd)
			{
				callback(user_data, sectionStart, sectionEnd);
				break;
			}
			else
			{
				size_t sectionLength = (char*)nextFreeBlock - sectionStart;

				if (sectionLength > 0)
					callback(user_data, sectionStart, sectionStart + sectionLength);

				sectionStart = (char*)nextFreeBlock + HDR(nextFreeBlock)->hb_sz;
			}
		}
	}
}

void HeapSectionCountIncrementer(void* context, GC_PTR start, GC_PTR end)
{
	GC_word* countPtr = (GC_word*)context;
	(*countPtr)++;
}

GC_word GC_get_heap_section_count()
{
	GC_word count = 0;
	GC_foreach_heap_section(&count, HeapSectionCountIncrementer);
	return count;
}